using HalconDotNet;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionDemo2026
{
    public class HalconCameraHelper
    {
        private static readonly object CameraSyncRoot = new object();
        private static readonly object CameraHandleSyncRoot = new object();
        private static CancellationTokenSource GrabCts;
        private static Task GrabTask;
        private static Task DetectionTask;
        private static LatestImageSlot LatestFrameSlot;
        private static HTuple ActiveHandle;
        private static Action<HImage> DetectionCallback;
        private static Action<CameraState, string> StateCallback;
        private static string CurrentCameraName;
        private static string CurrentInterfaceName;
        // 避免设备或驱动异常时 GrabImageAsync 无限阻塞，保证停止和重连有机会收敛。
        private const int GrabTimeoutMilliseconds = 1000;

        private static SimpleJsonHelper _jsonHelper = new SimpleJsonHelper();

        public enum CameraState
        {
            Stopped,
            Opening,
            Running,
            DeviceFailure,
            Reconnecting,
            Failed,
            UserStopped
        }

        public sealed class LatestImageSlot : IDisposable
        {
            private readonly object syncRoot = new object();
            private HImage image;

            public void Publish(HImage newImage)
            {
                if (newImage == null) return;

                HImage oldImage;
                lock (syncRoot)
                {
                    oldImage = image;
                    image = newImage;
                }
                if (oldImage != null) oldImage.Dispose();
            }

            public HImage Take()
            {
                lock (syncRoot)
                {
                    HImage result = image;
                    image = null;
                    return result;
                }
            }

            public void Clear()
            {
                HImage oldImage;
                lock (syncRoot)
                {
                    oldImage = image;
                    image = null;
                }
                if (oldImage != null) oldImage.Dispose();
            }

            public void Dispose()
            {
                Clear();
            }
        }

        public class CameraDeviceInfo
        {
            public string DisplayName { get; set; }
            public string DeviceName { get; set; }

            /// <summary>
            /// 返回相机在下拉框中显示的名称。
            /// </summary>
            /// <returns>相机显示名称。</returns>
            public override string ToString()
            {
                return DisplayName;
            }
        }

        #region 搜索相机
        /// <summary>
        /// 搜索指定接口下的相机，并仅返回设备显示名称。
        /// </summary>
        /// <param name="interfaceName">HALCON 相机接口名称，为空时使用 MediaFoundation。</param>
        /// <returns>搜索到的相机显示名称数组。</returns>
        public static string[] FindCamera(string interfaceName = null)
        {
            CameraDeviceInfo[] devices = FindCameraDevices(interfaceName);
            string[] cameraNames = new string[devices.Length];
            for (int i = 0; i < devices.Length; i++)
                cameraNames[i] = devices[i].DisplayName;

            return cameraNames;
        }

        /// <summary>
        /// 搜索指定接口下的相机，并保留显示名称和 HALCON 原始设备描述。
        /// </summary>
        /// <param name="interfaceName">HALCON 相机接口名称。</param>
        /// <returns>可用于界面显示和设备打开的相机信息数组。</returns>
        public static CameraDeviceInfo[] FindCameraDevices(string interfaceName)
        {
            if (string.IsNullOrWhiteSpace(interfaceName))
                interfaceName = "MediaFoundation";

            HOperatorSet.InfoFramegrabber(interfaceName, "info_boards", out HTuple information, out HTuple values);

            List<CameraDeviceInfo> cameras = new List<CameraDeviceInfo>();
            for (int i = 0; i < values.Length; i++)
            {
                string deviceName = values[i].S;
                string displayName = GetTokenValue(deviceName, "user_name");
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = GetTokenValue(deviceName, "unique_name");
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = GetTokenValue(deviceName, "device");
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = "相机 " + (i + 1);
                if (string.Equals(interfaceName, "MediaFoundation", StringComparison.OrdinalIgnoreCase))
                    displayName = GetMediaFoundationDisplayName(displayName);

                // HALCON 返回的完整设备描述可直接用于 open_framegrabber，适用于不同接口。
                cameras.Add(new CameraDeviceInfo
                {
                    DisplayName = displayName,
                    DeviceName = deviceName
                });
            }
            return cameras.ToArray();
        }
        #endregion

        #region 打开相机
        /// <summary>
        /// 使用指定接口和设备描述打开 HALCON 图像采集设备。
        /// </summary>
        /// <param name="interfaceName">HALCON 相机接口名称。</param>
        /// <param name="deviceName">HALCON 原始设备描述或兼容设备名。</param>
        /// <param name="acqHandle">输出打开后的图像采集句柄。</param>
        public static void Open(string interfaceName, string deviceName, out HTuple acqHandle)
        {
            HOperatorSet.OpenFramegrabber(interfaceName,
                0, 0, 0, 0, 0, 0,
                "progressive",
                -1,
                "default",
                -1,
                "false",
                "default",
                deviceName,
                0,
                -1,
                out acqHandle);
        }
        #endregion

        #region 抓一帧（同步）
        #region 打开指定相机
        /// <summary>
        /// 按相机名称打开相机，已打开同一相机时复用句柄。
        /// </summary>
        /// <param name="interfaceName">HALCON 相机接口名称。</param>
        /// <param name="cameraName">HALCON 原始设备描述或兼容的相机名称。</param>
        /// <param name="acqHandle">相机句柄引用，必要时由方法重新打开并替换。</param>
        /// <param name="message">输出相机打开失败时的中文提示。</param>
        /// <returns>相机成功打开或已有句柄可以复用时返回 true。</returns>
        public static bool OpenCameraByName(string interfaceName, string cameraName, ref HTuple acqHandle, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(cameraName))
            {
                message = "\u8bf7\u5148\u9009\u62e9\u76f8\u673a";
                return false;
            }

            try
            {
                if (!IsOpen(acqHandle) || CurrentCameraName != cameraName ||
                    CurrentInterfaceName != interfaceName)
                {
                    StopGrab(ref acqHandle);
                    if (IsOpen(acqHandle))
                    {
                        try { HOperatorSet.CloseFramegrabber(acqHandle); } catch { }
                        acqHandle = null;
                    }
                    string deviceName = cameraName;
                    if (string.Equals(interfaceName, "GigEVision2", StringComparison.OrdinalIgnoreCase) &&
                        cameraName.IndexOf("|", StringComparison.Ordinal) < 0 &&
                        cameraName.IndexOf("user_name:", StringComparison.OrdinalIgnoreCase) < 0 &&
                        cameraName.IndexOf("device:", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        // 兼容旧配置中只保存 GigE 相机 user_name 的情况。
                        deviceName = "user_name:" + cameraName;
                    }

                    Open(interfaceName, deviceName, out acqHandle);
                    CurrentCameraName = cameraName;
                    CurrentInterfaceName = interfaceName;
                }

                return true;
            }
            catch (Exception ex)
            {
                message = "\u76f8\u673a\u6253\u5f00\u5931\u8d25\uff1a" + ex.Message;
                return false;
            }
        }
        #endregion

        #region 单次采集
        /// <summary>
        /// 打开相机，按需应用曝光，并采集一张图显示到窗口。
        /// </summary>
        /// <param name="interfaceName">HALCON 相机接口名称。</param>
        /// <param name="cameraName">HALCON 原始设备描述或兼容的相机名称。</param>
        /// <param name="exposureText">界面输入的曝光值文本，为空时保持相机当前曝光。</param>
        /// <param name="window">用于显示采集图像的 HALCON 窗口控件。</param>
        /// <param name="acqHandle">相机句柄引用，必要时由方法重新打开并替换。</param>
        /// <param name="message">输出采集失败时的中文提示。</param>
        /// <returns>图像采集并显示成功时返回 true。</returns>
        public static bool CaptureSingleImage(string interfaceName, string cameraName, string exposureText, HWindowControl window, ref HTuple acqHandle, out string message)
        {
            HImage cameraImage;
            if (!CaptureSingleImage(interfaceName, cameraName, exposureText,
                ref acqHandle, out cameraImage, out message))
                return false;

            try
            {
                DisplayImageFitWindow(window, cameraImage);
                return true;
            }
            finally
            {
                cameraImage.Dispose();
            }
        }

        /// <summary>
        /// 打开相机、按需应用曝光并采集一张由调用方管理的图像。
        /// </summary>
        /// <param name="interfaceName">HALCON 相机接口名称。</param>
        /// <param name="cameraName">HALCON 原始设备描述或兼容的相机名称。</param>
        /// <param name="exposureText">界面输入的曝光值文本，为空时保持相机当前曝光。</param>
        /// <param name="acqHandle">相机句柄引用，必要时由方法重新打开并替换。</param>
        /// <param name="cameraImage">输出采集图像，由调用方负责释放。</param>
        /// <param name="message">输出采集失败时的中文提示。</param>
        /// <returns>图像采集成功时返回 true。</returns>
        public static bool CaptureSingleImage(string interfaceName, string cameraName,
            string exposureText, ref HTuple acqHandle, out HImage cameraImage, out string message)
        {
            cameraImage = null;
            StopGrab(ref acqHandle);
            if (!OpenCameraByName(interfaceName, cameraName, ref acqHandle, out message))
                return false;

            if (!ApplyExposureText(acqHandle, exposureText, false, out message))
                return false;

            try
            {
                cameraImage = Grab(acqHandle);
                return true;
            }
            catch (Exception ex)
            {
                if (cameraImage != null)
                {
                    cameraImage.Dispose();
                    cameraImage = null;
                }
                message = "\u56fe\u50cf\u91c7\u96c6\u5931\u8d25\uff1a" + ex.Message;
                return false;
            }
        }
        #endregion

        public static HImage Grab(HTuple acqHandle)
        {
            HObject image;
            HOperatorSet.GrabImage(out image, acqHandle);
            return new HImage(image);
        }

        public static HImage GrabAsync(HTuple acqHandle)
        {
            HObject image;
            HOperatorSet.GrabImageAsync(out image, acqHandle, GrabTimeoutMilliseconds);
            return new HImage(image);
        }

        public static bool IsOpen(HTuple acqHandle)
        {
            return acqHandle != null && acqHandle.Length > 0;
        }

        public static void Close(ref HTuple acqHandle)
        {
            StopGrab(ref acqHandle);
            try
            {
                if (IsOpen(acqHandle))
                    HOperatorSet.CloseFramegrabber(acqHandle);
            }
            finally
            {
                acqHandle = null;
                CurrentCameraName = null;
                CurrentInterfaceName = null;
            }
        }

        #region 连续采集
        public static bool StartContinuousGrab(string interfaceName, string cameraName, string exposureText,
            HWindowControl window, Action<HImage> imageReceived, ref HTuple acqHandle, out string message)
        {
            return StartContinuousGrab(interfaceName, cameraName, exposureText, window,
                imageReceived, null, null, ref acqHandle, out message);
        }

        public static bool StartContinuousGrab(string interfaceName, string cameraName, string exposureText,
            HWindowControl window, Action<HImage> imageReceived, Action<HImage> detectionReceived,
            Action<CameraState, string> stateChanged, ref HTuple acqHandle, out string message)
        {
            message = string.Empty;
            StopGrab(ref acqHandle);
            NotifyState(stateChanged, CameraState.Opening, "正在打开相机");
            if (!OpenCameraByName(interfaceName, cameraName, ref acqHandle, out message))
                return false;
            if (!ApplyExposureText(acqHandle, exposureText, false, out message))
            {
                Close(ref acqHandle);
                return false;
            }

            try
            {
                HOperatorSet.GrabImageStart(acqHandle, -1);
                lock (CameraSyncRoot)
                {
                    ActiveHandle = acqHandle;
                    GrabCts = new CancellationTokenSource();
                    CancellationToken token = GrabCts.Token;
                    LatestFrameSlot = new LatestImageSlot();
                    DetectionCallback = detectionReceived;
                    StateCallback = stateChanged;
                    GrabTask = Task.Run(() => AcquisitionLoop(interfaceName, cameraName, exposureText,
                        imageReceived, token));
                    DetectionTask = detectionReceived == null
                        ? null
                        : Task.Run(() => DetectionLoop(token));
                }
                NotifyState(CameraState.Running, "连续采集已启动");
                return true;
            }
            catch (Exception ex)
            {
                message = "连续采集启动失败：" + ex.Message;
                StopGrab();
                Close(ref acqHandle);
                return false;
            }
        }

        public static void StartGrab(HTuple acqHandle, HWindowControl window)
        {
            StartGrab(acqHandle, window, null);
        }

        public static void StartGrab(HTuple acqHandle, HWindowControl window, Action<HImage> imageReceived)
        {
            string message;
            StartContinuousGrab(CurrentInterfaceName, CurrentCameraName, string.Empty,
                window, imageReceived, null, null, ref acqHandle, out message);
        }

        public static void StopGrab()
        {
            CancellationTokenSource cts;
            Task grabTask;
            Task detectionTask;
            LatestImageSlot slot;
            Action<CameraState, string> stateChanged;
            lock (CameraSyncRoot)
            {
                cts = GrabCts;
                grabTask = GrabTask;
                detectionTask = DetectionTask;
                slot = LatestFrameSlot;
                stateChanged = StateCallback;
                GrabCts = null;
                GrabTask = null;
                DetectionTask = null;
                LatestFrameSlot = null;
                DetectionCallback = null;
                StateCallback = null;
            }
            if (cts == null) return;

            cts.Cancel();
            WaitTask(grabTask);
            WaitTask(detectionTask);
            if (slot != null) slot.Dispose();
            cts.Dispose();
            CloseActiveHandle();
            NotifyState(stateChanged, CameraState.UserStopped, "用户已停止连续采集");
        }

        public static void StopGrab(ref HTuple acqHandle)
        {
            bool hadContinuousGrab;
            lock (CameraSyncRoot)
            {
                hadContinuousGrab = GrabCts != null;
            }
            StopGrab();
            // 连续会话的活动句柄已由 StopGrab() 在等待任务退出后关闭，避免重复关闭。
            if (!hadContinuousGrab && IsOpen(acqHandle))
            {
                try { HOperatorSet.CloseFramegrabber(acqHandle); } catch { }
            }
            acqHandle = null;
        }

        private static void AcquisitionLoop(string interfaceName, string cameraName, string exposureText,
            Action<HImage> imageReceived, CancellationToken token)
        {
            const int failureThreshold = 3;
            const int maxReconnectAttempts = 5;
            int consecutiveFailures = 0;
            int reconnectAttempts = 0;

            // 当前 HALCON 引用未提供可验证的设备丢失事件，这里以连续抓图失败作为断线判据。
            while (!token.IsCancellationRequested)
            {
                HImage image = null;
                try
                {
                    image = GrabActiveAsync();
                    consecutiveFailures = 0;
                    reconnectAttempts = 0;

                    HImage previewImage = image.CopyImage();
                    if (imageReceived != null)
                        imageReceived(previewImage);
                    else
                        previewImage.Dispose();

                    LatestImageSlot slot = LatestFrameSlot;
                    if (slot != null)
                    {
                        slot.Publish(image);
                        image = null;
                    }
                }
                catch (Exception ex)
                {
                    if (token.IsCancellationRequested) break;

                    consecutiveFailures++;
                    if (consecutiveFailures < failureThreshold)
                    {
                        if (token.WaitHandle.WaitOne(50)) break;
                        continue;
                    }

                    NotifyState(CameraState.DeviceFailure, "连续采集失败：" + ex.Message);
                    if (reconnectAttempts >= maxReconnectAttempts)
                    {
                        NotifyState(CameraState.Failed, "相机重连失败，已达到最大尝试次数");
                        CleanupFailedSession();
                        break;
                    }

                    reconnectAttempts++;
                    int delay = Math.Min(4000, 250 * (1 << Math.Min(reconnectAttempts - 1, 4)));
                    NotifyState(CameraState.Reconnecting,
                        "相机重连中（" + reconnectAttempts + "/" + maxReconnectAttempts + "）");
                    if (token.WaitHandle.WaitOne(delay)) break;

                    if (token.IsCancellationRequested)
                        break;

                    try
                    {
                        ReopenActiveHandle(interfaceName, cameraName, exposureText, token);
                        consecutiveFailures = 0;
                        reconnectAttempts = 0;
                        NotifyState(CameraState.Running, "相机重连成功");
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception reconnectException)
                    {
                        NotifyState(CameraState.DeviceFailure, "相机重连失败：" + reconnectException.Message);
                    }
                }
                finally
                {
                    if (image != null) image.Dispose();
                }
            }
        }

        private static void DetectionLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                LatestImageSlot slot = LatestFrameSlot;
                HImage image = slot == null ? null : slot.Take();
                if (image == null)
                {
                    if (token.WaitHandle.WaitOne(20)) break;
                    continue;
                }

                Action<HImage> callback = DetectionCallback;
                if (callback == null)
                {
                    image.Dispose();
                    continue;
                }

                try
                {
                    callback(image);
                }
                catch
                {
                    // 回调异常由 finally 统一释放当前帧，避免重复 Dispose。
                }
                finally
                {
                    image.Dispose();
                }
            }
        }

        private static void ReopenActiveHandle(string interfaceName, string cameraName, string exposureText,
            CancellationToken token)
        {
            lock (CameraHandleSyncRoot)
            {
                if (token.IsCancellationRequested)
                    throw new OperationCanceledException(token);

                CloseActiveHandle();
                string deviceName = GetCompatibleDeviceName(interfaceName, cameraName);
                HTuple reopenedHandle;
                Open(interfaceName, deviceName, out reopenedHandle);
                try
                {
                    if (token.IsCancellationRequested)
                        throw new OperationCanceledException(token);
                    if (!ApplyExposureText(reopenedHandle, exposureText, false, out string exposureMessage))
                        throw new InvalidOperationException(exposureMessage);
                    HOperatorSet.GrabImageStart(reopenedHandle, -1);
                    if (token.IsCancellationRequested)
                        throw new OperationCanceledException(token);
                    ActiveHandle = reopenedHandle;
                    reopenedHandle = null;
                }
                catch
                {
                    try
                    {
                        if (IsOpen(reopenedHandle))
                            HOperatorSet.CloseFramegrabber(reopenedHandle);
                    }
                    catch
                    {
                    }
                    throw;
                }
            }
        }

        private static void CleanupFailedSession()
        {
            CancellationTokenSource cts;
            Task detectionTask;
            LatestImageSlot slot;
            lock (CameraSyncRoot)
            {
                cts = GrabCts;
                detectionTask = DetectionTask;
                slot = LatestFrameSlot;
                GrabCts = null;
                GrabTask = null;
                DetectionTask = null;
                LatestFrameSlot = null;
                DetectionCallback = null;
                StateCallback = null;
            }

            if (cts != null)
                cts.Cancel();
            // 当前方法由采集线程调用，不能等待 GrabTask 自身；当前迭代已离开 HALCON 调用。
            WaitTask(detectionTask);
            try
            {
                if (slot != null)
                    slot.Dispose();
            }
            finally
            {
                try
                {
                    CloseActiveHandle();
                }
                catch
                {
                    // 设备已断开时关闭句柄可能失败，但不能阻断其余会话清理。
                }
                finally
                {
                    if (cts != null)
                        cts.Dispose();
                }
            }
        }

        private static HImage GrabActiveAsync()
        {
            lock (CameraHandleSyncRoot)
            {
                if (!IsOpen(ActiveHandle))
                    throw new InvalidOperationException("相机采集句柄无效。");
                return GrabAsync(ActiveHandle);
            }
        }

        private static string GetCompatibleDeviceName(string interfaceName, string cameraName)
        {
            if (string.Equals(interfaceName, "GigEVision2", StringComparison.OrdinalIgnoreCase) &&
                cameraName.IndexOf("|", StringComparison.Ordinal) < 0 &&
                cameraName.IndexOf("user_name:", StringComparison.OrdinalIgnoreCase) < 0 &&
                cameraName.IndexOf("device:", StringComparison.OrdinalIgnoreCase) < 0)
                return "user_name:" + cameraName;
            return cameraName;
        }

        private static void CloseActiveHandle()
        {
            lock (CameraHandleSyncRoot)
            {
                HTuple handle = ActiveHandle;
                ActiveHandle = null;
                if (IsOpen(handle))
                {
                    try
                    {
                        HOperatorSet.CloseFramegrabber(handle);
                    }
                    finally
                    {
                        handle = null;
                    }
                }
            }
        }

        private static void WaitTask(Task task)
        {
            if (task == null || task.IsCompleted) return;
            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
            }
        }

        private static void NotifyState(CameraState state, string message)
        {
            NotifyState(StateCallback, state, message);
        }

        private static void NotifyState(Action<CameraState, string> callback, CameraState state, string message)
        {
            if (callback == null) return;
            try
            {
                callback(state, message);
            }
            catch
            {
            }
        }
        #endregion
        #endregion

        #region 解析设备名
        /// <summary>
        /// 从 HALCON 设备信息文本中提取 device 字段。
        /// </summary>
        /// <param name="value">HALCON 返回的设备信息文本。</param>
        /// <returns>清理后的设备名称。</returns>
        private static string ParseDeviceName(string value)
        {
            value = value.Trim().Trim('"').Trim();

            int start = value.IndexOf("device:");
            if (start >= 0)
                value = value.Substring(start + 7);

            int end = value.IndexOf("unique_name");
            if (end >= 0)
                value = value.Substring(0, end);

            return value.Trim().Trim('|').Trim();
        }
        #endregion

        #region 显示图像（原图1:1像素）
        /// <summary>
        /// 按原始像素比例显示相机图像，并保存显示图像副本。
        /// </summary>
        /// <param name="window">目标 HALCON 窗口控件。</param>
        /// <param name="image">需要显示的相机图像。</param>
        public static void DisplayImage(HWindowControl window, HImage image)
        {
            if (window == null || image == null) return;

            HTuple width, height;
            HOperatorSet.GetImageSize(image, out width, out height);

            // 取图像和窗口的较大值，保证1:1像素显示
            int imgW = width.I;
            int imgH = height.I;
            int winW = Math.Max(1, window.Width);
            int winH = Math.Max(1, window.Height);
            int maxR = Math.Max(imgH, winH);
            int maxC = Math.Max(imgW, winW);

            window.HalconWindow.SetPart(
                new HTuple(0), new HTuple(0),
                new HTuple(maxR - 1), new HTuple(maxC - 1));
            window.HalconWindow.ClearWindow();
            window.HalconWindow.DispImage(image);

            HImage oldImage = window.Tag as HImage;
            if (oldImage != null)
                oldImage.Dispose();

            window.Tag = image.CopyImage();
        }
        #endregion

        #region 显示图像（缩放适应窗口）
        /// <summary>
        /// 将相机图像缩放至 HALCON 窗口范围内显示。
        /// </summary>
        /// <param name="window">目标 HALCON 窗口控件。</param>
        /// <param name="image">需要显示的相机图像。</param>
        public static void DisplayImageFitWindow(HWindowControl window, HImage image)
        {
            if (window == null || image == null) return;

            HTuple width, height;
            HOperatorSet.GetImageSize(image, out width, out height);

            // SetPart用图像尺寸，HALCON自动缩放填满窗口
            window.HalconWindow.SetPart(
                new HTuple(0), new HTuple(0),
                new HTuple(height.I - 1), new HTuple(width.I - 1));
            window.HalconWindow.ClearWindow();
            window.HalconWindow.DispImage(image);

            HImage oldImage = window.Tag as HImage;
            if (oldImage != null)
                oldImage.Dispose();

            window.Tag = image.CopyImage();
        }
        #endregion


        #region 设置相机曝光
        /// <summary>
        /// 关闭自动曝光，并尝试使用常见参数名设置手动曝光值。
        /// </summary>
        /// <param name="acqHandle">已经打开的 HALCON 图像采集句柄。</param>
        /// <param name="exposureValue">需要设置的正数曝光值。</param>
        /// <returns>至少一个曝光参数设置成功时返回 true。</returns>
        public static bool SetCameraExposure(HTuple acqHandle, double exposureValue)
        {
            if (acqHandle == null || acqHandle.Length == 0) return false;
            if (exposureValue <= 0) return false;

            try
            {
                // 关闭自动曝光，防止手动曝光值被相机自动覆盖
                TrySetFramegrabberParam(acqHandle, "[Consumer]exposure_auto", "Off");
                TrySetFramegrabberParam(acqHandle, "ExposureAuto", "Off");

                // 常见工业相机曝光参数名
                string[] exposureNames =
                {
                    "[Consumer]exposure",
                    "ExposureTime",
                    "ExposureTimeAbs",
                    "exposure"
                };

                // 不同相机接口的曝光参数名可能不同，所以逐个尝试
                foreach (string name in exposureNames)
                {
                    if (TrySetFramegrabberParam(acqHandle, name, exposureValue))
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 尝试设置相机参数
        #region 设置当前相机曝光
        /// <summary>
        /// 打开相机并设置曝光。
        /// </summary>
        /// <param name="interfaceName">HALCON 相机接口名称。</param>
        /// <param name="cameraName">HALCON 原始设备描述或兼容的相机名称。</param>
        /// <param name="exposureText">界面输入的曝光值文本。</param>
        /// <param name="acqHandle">相机句柄引用，必要时由方法重新打开并替换。</param>
        /// <param name="message">输出相机打开或曝光设置失败时的中文提示。</param>
        /// <returns>相机成功打开且曝光设置成功时返回 true。</returns>
        public static bool SetCameraExposureByName(string interfaceName, string cameraName, string exposureText, ref HTuple acqHandle, out string message)
        {
            StopGrab(ref acqHandle);
            if (!OpenCameraByName(interfaceName, cameraName, ref acqHandle, out message))
                return false;

            return ApplyExposureText(acqHandle, exposureText, true, out message);
        }
        #endregion

        #region 应用曝光文本
        /// <summary>
        /// 解析曝光文本，并按是否必填决定空值处理方式。
        /// </summary>
        /// <param name="acqHandle">已经打开的 HALCON 图像采集句柄。</param>
        /// <param name="exposureText">需要解析和应用的曝光值文本。</param>
        /// <param name="required">是否要求必须输入有效曝光值。</param>
        /// <param name="message">输出曝光值无效或设置失败时的中文提示。</param>
        /// <returns>无需设置或曝光设置成功时返回 true。</returns>
        private static bool ApplyExposureText(HTuple acqHandle, string exposureText, bool required, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(exposureText))
            {
                if (required)
                {
                    message = "\u8bf7\u8f93\u5165\u6b63\u786e\u7684\u66dd\u5149\u503c";
                    return false;
                }

                return true;
            }

            if (!double.TryParse(exposureText, out double exposure) || exposure <= 0)
            {
                message = "\u8bf7\u8f93\u5165\u6b63\u786e\u7684\u66dd\u5149\u503c";
                return false;
            }

            if (!SetCameraExposure(acqHandle, exposure))
            {
                message = "\u66dd\u5149\u8bbe\u7f6e\u5931\u8d25\uff0c\u8bf7\u68c0\u67e5\u76f8\u673a\u662f\u5426\u652f\u6301\u8be5\u66dd\u5149\u53c2\u6570";
                return false;
            }

            return true;
        }
        #endregion

        /// <summary>
        /// 尝试设置一个 HALCON 图像采集参数，并屏蔽设备不支持该参数的异常。
        /// </summary>
        /// <param name="acqHandle">已经打开的 HALCON 图像采集句柄。</param>
        /// <param name="paramName">需要设置的采集参数名称。</param>
        /// <param name="value">需要写入的参数值。</param>
        /// <returns>参数设置成功时返回 true。</returns>
        private static bool TrySetFramegrabberParam(HTuple acqHandle, string paramName, HTuple value)
        {
            try
            {
                // 设置相机参数
                HOperatorSet.SetFramegrabberParam(acqHandle, paramName, value);
                return true;
            }
            catch
            {
                // 当前相机不支持该参数时会进入这里
                return false;
            }
        }
        #endregion


        /// <summary>
        /// 按旧版 GigE 配置格式保存当前相机名称和曝光值。
        /// </summary>
        /// <param name="CameraConfigFolder">配置文件夹名称。</param>
        /// <param name="CameraConfigName">不含扩展名的配置文件名。</param>
        /// <param name="CameraName">需要保存的 GigE 相机名称。</param>
        /// <param name="Exposure">需要保存的曝光值文本。</param>
        public static void SaveCameraConfig(string CameraConfigFolder, string CameraConfigName,string CameraName,string Exposure)
        {
            SaveCameraConfig(CameraConfigFolder, CameraConfigName, "GigEVision2",
                CameraName, CameraName, Exposure);
        }

        /// <summary>
        /// 保存相机接口、显示名称、设备标识和曝光值。
        /// </summary>
        /// <param name="cameraConfigFolder">配置文件夹名称。</param>
        /// <param name="cameraConfigName">不含扩展名的配置文件名。</param>
        /// <param name="interfaceName">HALCON 相机接口名称。</param>
        /// <param name="cameraName">供界面显示的相机名称。</param>
        /// <param name="deviceName">HALCON 原始设备描述。</param>
        /// <param name="exposure">需要保存的曝光值文本。</param>
        public static void SaveCameraConfig(string cameraConfigFolder, string cameraConfigName,
            string interfaceName, string cameraName, string deviceName, string exposure)
        {
            SimpleJsonHelper.Information config = new SimpleJsonHelper.Information
            {
                CameraInterfaceName = interfaceName,
                CamerName = cameraName,
                CameraDeviceName = deviceName,
                Exposure = (exposure ?? string.Empty).Trim()
            };

            _jsonHelper.JsonWrite(cameraConfigFolder, cameraConfigName, config);
        }

        /// <summary>
        /// 从竖线分隔的 HALCON 设备信息中读取指定字段值。
        /// </summary>
        /// <param name="deviceInformation">HALCON 返回的完整设备信息。</param>
        /// <param name="tokenName">需要读取的字段名称，不含冒号。</param>
        /// <returns>字段值，字段不存在时返回空字符串。</returns>
        private static string GetTokenValue(string deviceInformation, string tokenName)
        {
            if (string.IsNullOrWhiteSpace(deviceInformation)) return string.Empty;

            string token = tokenName + ":";
            int start = deviceInformation.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (start < 0) return string.Empty;

            start += token.Length;
            int end = deviceInformation.IndexOf("|", start, StringComparison.Ordinal);
            if (end < 0) end = deviceInformation.Length;
            return deviceInformation.Substring(start, end - start).Trim();
        }

        /// <summary>
        /// 清理 MediaFoundation 设备名称中的硬件 ID 和下划线。
        /// </summary>
        /// <param name="displayName">MediaFoundation 返回的原始显示名称。</param>
        /// <returns>适合在界面中展示的相机名称。</returns>
        public static string GetMediaFoundationDisplayName(string displayName)
        {
            // MediaFoundation 的 unique_name 常附带 VID/PID，界面只显示便于识别的设备名称。
            int hardwareIdStart = displayName.IndexOf("_vid_", StringComparison.OrdinalIgnoreCase);
            if (hardwareIdStart > 0)
                displayName = displayName.Substring(0, hardwareIdStart);

            return displayName.Replace('_', ' ').Trim();
        }
    }
}
