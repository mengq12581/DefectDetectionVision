using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using Sunny.UI;

namespace VisionDemo2026
{
    public partial class Form1 : UIForm
    {
        private HImage currentImage;
        private HImage adjustedImage;
        private HImage maskImage;
        private HImage edgeImage;
        private HObject roiRegion;
        private List<DetectionItem> detectionItems = new List<DetectionItem>();
        private readonly SimpleJsonHelper jsonHelper = new SimpleJsonHelper();
        private HTuple cameraHandle;
        private bool isContinuousGrabbing;
        private bool isLoadingCameraConfig = true;

        private const string ConfigFolderName = "Config";
        private const string ImageSourceConfigName = "LastImageSource";
        private const string CameraConfigName = "CameraConfig";

        // 相机接口名称: 1、 MediaFoundation 为笔记本相机   2、 GigEVision2 为网络相机
        private const string LocalCameraInterfaceName = "MediaFoundation";
        private const string NetworkCameraInterfaceName = "GigEVision2";

        private string CameraInterfaceName
        {
            get
            {
                return cmbCameraInterface.SelectedIndex == 1
                    ? NetworkCameraInterfaceName
                    : LocalCameraInterfaceName;
            }
        }

        /// <summary>
        /// 初始化主窗体、相机接口选项以及窗体生命周期事件。
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            if (cmbCameraInterface.SelectedIndex < 0)
                cmbCameraInterface.SelectedIndex = 0;
            FormClosed += Form1_FormClosed;
            Shown += Form1_Shown;
        }

        #region 图像采集
        private string SelectedCameraName
        {
            get
            {
                HalconCameraHelper.CameraDeviceInfo camera =
                    cmbCamera.SelectedItem as HalconCameraHelper.CameraDeviceInfo;
                return camera == null ? null : camera.DisplayName;
            }
        }

        private string SelectedCameraDeviceName
        {
            get
            {
                HalconCameraHelper.CameraDeviceInfo camera =
                    cmbCamera.SelectedItem as HalconCameraHelper.CameraDeviceInfo;
                return camera == null ? null : camera.DeviceName;
            }
        }

        /// <summary>
        /// 响应搜索相机按钮，枚举当前接口下可用的相机设备。
        /// </summary>
        private void btnSearchCamera_Click(object sender, EventArgs e)
        {
            FindCameras(true);
        }

        /// <summary>
        /// 响应单次采集按钮，采集一帧相机图像并立即执行检测。
        /// </summary>
        private void btnCaptureSingle_Click(object sender, EventArgs e)
        {
            StopContinuousGrab(false);

            HImage cameraImage;
            string message;
            if (!HalconCameraHelper.CaptureSingleImage(CameraInterfaceName, SelectedCameraDeviceName,
                txtExposure.Text, ref cameraHandle, out cameraImage, out message))
            {
                MessageBox.Show(this, message, "采集失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 返回图像的所有权交给窗体，后续统一在切图或关闭窗体时释放。
            UseCameraImage(cameraImage);
            SaveCameraConfig();
            RunDetection();
        }

        /// <summary>
        /// 响应连续采集按钮，清理旧结果并启动实时图像预览。
        /// </summary>
        private void btnStartGrab_Click(object sender, EventArgs e)
        {
            if (isContinuousGrabbing) return;

            ClearRoi();
            DisposeResultImages();
            detectionItems.Clear();
            HalconImageHelper.ClearWindow(hWindowMask);
            HalconImageHelper.ClearWindow(hWindowEdge);
            HalconImageHelper.SetCameraImageMode(SelectedCameraName);
            UpdateNavigationState();
            lblSummary.Text = "连续采集中，停止采集后检测最后一帧。";

            string message;
            if (!HalconCameraHelper.StartContinuousGrab(CameraInterfaceName, SelectedCameraDeviceName,
                txtExposure.Text, hWindowAnalysis, CameraFrameReceived,
                ref cameraHandle, out message))
            {
                MessageBox.Show(this, message, "采集失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateCameraControlState();
                return;
            }

            isContinuousGrabbing = true;
            SaveCameraConfig();
            UpdateCameraControlState();
            UpdateSourceText();
        }

        /// <summary>
        /// 响应停止采集按钮，停止取流并检测最后一帧图像。
        /// </summary>
        private void btnStopGrab_Click(object sender, EventArgs e)
        {
            StopContinuousGrab(true);
        }

        /// <summary>
        /// 响应设置曝光按钮，将输入的曝光值应用到当前相机。
        /// </summary>
        private void btnSetExposure_Click(object sender, EventArgs e)
        {
            string message;
            if (!HalconCameraHelper.SetCameraExposureByName(CameraInterfaceName,
                SelectedCameraDeviceName, txtExposure.Text, ref cameraHandle, out message))
            {
                MessageBox.Show(this, message, "设置失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveCameraConfig();
            MessageBox.Show(this, "曝光设置成功。", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 响应相机选择变化，刷新采集控件状态并保存相机配置。
        /// </summary>
        private void cmbCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCameraControlState();
            if (!isLoadingCameraConfig)
                SaveCameraConfig();
        }

        /// <summary>
        /// 响应相机接口变化，关闭旧设备并清空不能复用的设备列表。
        /// </summary>
        private void cmbCameraInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingCameraConfig) return;

            StopContinuousGrab(false);
            try
            {
                HalconCameraHelper.Close(ref cameraHandle);
            }
            catch (Exception ex)
            {
                lblSource.Text = "切换相机接口时关闭设备失败：" + ex.Message;
            }

            // 不同接口返回的设备标识不能混用，切换后需要重新搜索。
            cmbCamera.Items.Clear();
            SaveCameraConfig();
            UpdateCameraControlState();
        }

        /// <summary>
        /// 搜索当前接口下的相机，并将设备信息填入相机下拉框。
        /// </summary>
        /// <param name="showMessage">是否弹窗显示搜索结果。</param>
        private void FindCameras(bool showMessage)
        {
            string previousDeviceName = SelectedCameraDeviceName;

            try
            {
                HalconCameraHelper.CameraDeviceInfo[] cameras =
                    HalconCameraHelper.FindCameraDevices(CameraInterfaceName);
                cmbCamera.Items.Clear();
                foreach (HalconCameraHelper.CameraDeviceInfo camera in cameras)
                {
                    cmbCamera.Items.Add(camera);
                }

                SelectCamera(null, previousDeviceName);
                if (cmbCamera.SelectedIndex < 0 && cmbCamera.Items.Count > 0)
                    cmbCamera.SelectedIndex = 0;

                if (showMessage)
                {
                    string cameraType = CameraInterfaceName == LocalCameraInterfaceName
                        ? "笔记本相机"
                        : "网络相机";
                    string message = cmbCamera.Items.Count == 0
                        ? "未搜索到" + cameraType + "，请检查设备连接及系统相机权限。"
                        : "已搜索到 " + cmbCamera.Items.Count + " 台" + cameraType + "。";
                    MessageBox.Show(this, message, "搜索相机",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                cmbCamera.Items.Clear();
                if (showMessage)
                {
                    MessageBox.Show(this, "搜索相机失败：" + ex.Message, "搜索相机",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            UpdateCameraControlState();
        }

        /// <summary>
        /// 从配置文件恢复相机接口、设备标识和曝光值。
        /// </summary>
        private void LoadCameraConfig()
        {
            try
            {
                SimpleJsonHelper.Information config =
                    jsonHelper.JsonRead(ConfigFolderName, CameraConfigName);
                if (config == null) return;

                if (!string.IsNullOrWhiteSpace(config.Exposure))
                    txtExposure.Text = config.Exposure;

                string savedInterfaceName = config.CameraInterfaceName;
                if (string.IsNullOrWhiteSpace(savedInterfaceName) &&
                    !string.IsNullOrWhiteSpace(config.CamerName))
                {
                    // 旧版配置只支持 GigE，相机接口字段为空时按网络相机兼容。
                    savedInterfaceName = NetworkCameraInterfaceName;
                }

                cmbCameraInterface.SelectedIndex =
                    savedInterfaceName == NetworkCameraInterfaceName ? 1 : 0;

                // 保存原始设备描述后，启动时无需重新枚举也能直接打开上次使用的设备。
                if (!string.IsNullOrWhiteSpace(config.CamerName))
                {
                    string savedCameraName = config.CamerName;
                    if (savedInterfaceName == LocalCameraInterfaceName)
                    {
                        savedCameraName =
                            HalconCameraHelper.GetMediaFoundationDisplayName(savedCameraName);
                    }

                    SelectCamera(savedCameraName, config.CameraDeviceName);
                    if (cmbCamera.SelectedIndex < 0)
                    {
                        cmbCamera.Items.Add(new HalconCameraHelper.CameraDeviceInfo
                        {
                            DisplayName = savedCameraName,
                            DeviceName = string.IsNullOrWhiteSpace(config.CameraDeviceName)
                                ? config.CamerName
                                : config.CameraDeviceName
                        });
                        cmbCamera.SelectedIndex = cmbCamera.Items.Count - 1;
                    }
                }

            }
            catch (Exception ex)
            {
                lblSource.Text = "相机配置加载失败：" + ex.Message;
            }
        }

        /// <summary>
        /// 将当前相机接口、设备标识和曝光值保存到配置文件。
        /// </summary>
        private void SaveCameraConfig()
        {
            try
            {
                HalconCameraHelper.SaveCameraConfig(ConfigFolderName, CameraConfigName,
                    CameraInterfaceName, SelectedCameraName, SelectedCameraDeviceName,
                    txtExposure.Text);
            }
            catch (Exception ex)
            {
                lblSource.Text = "相机配置保存失败：" + ex.Message;
            }
        }

        /// <summary>
        /// 根据显示名称或 HALCON 设备标识选中相机列表中的设备。
        /// </summary>
        /// <param name="cameraName">供界面显示的相机名称。</param>
        /// <param name="deviceName">HALCON 返回的原始设备标识。</param>
        private void SelectCamera(string cameraName, string deviceName)
        {
            if (string.IsNullOrWhiteSpace(cameraName) && string.IsNullOrWhiteSpace(deviceName))
                return;

            for (int i = 0; i < cmbCamera.Items.Count; i++)
            {
                HalconCameraHelper.CameraDeviceInfo camera =
                    cmbCamera.Items[i] as HalconCameraHelper.CameraDeviceInfo;
                if (camera == null) continue;

                bool sameDevice = !string.IsNullOrWhiteSpace(deviceName) &&
                    string.Equals(camera.DeviceName, deviceName, StringComparison.OrdinalIgnoreCase);
                bool sameName = !string.IsNullOrWhiteSpace(cameraName) &&
                    string.Equals(camera.DisplayName, cameraName, StringComparison.OrdinalIgnoreCase);
                if (sameDevice || sameName)
                {
                    cmbCamera.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// 根据相机选择和连续采集状态启用或禁用采集控件。
        /// </summary>
        private void UpdateCameraControlState()
        {
            bool hasCamera = !string.IsNullOrWhiteSpace(SelectedCameraName);
            cmbCameraInterface.Enabled = !isContinuousGrabbing;
            cmbCamera.Enabled = !isContinuousGrabbing;
            btnSearchCamera.Enabled = !isContinuousGrabbing;
            txtExposure.Enabled = !isContinuousGrabbing;
            btnSetExposure.Enabled = hasCamera && !isContinuousGrabbing;
            btnCaptureSingle.Enabled = hasCamera && !isContinuousGrabbing;
            btnStartGrab.Enabled = hasCamera && !isContinuousGrabbing;
            btnStopGrab.Enabled = isContinuousGrabbing;
        }

        /// <summary>
        /// 接收连续采集的新图像，替换当前帧并刷新预览窗口。
        /// </summary>
        /// <param name="image">相机采集线程传回的 HALCON 图像。</param>
        private void CameraFrameReceived(HImage image)
        {
            // 停止后可能还有一帧已排入 UI 队列，此时直接忽略，避免覆盖检测结果。
            if (!isContinuousGrabbing || image == null) return;

            HImage latestImage = image.CopyImage();
            HalconImageHelper.SafeDispose(ref currentImage);
            currentImage = latestImage;
            HalconImageHelper.SetCameraImageMode(SelectedCameraName);
            HalconImageHelper.DisplayImageFitWindow(hWindowAnalysis, currentImage);
        }

        /// <summary>
        /// 将单次采集图像设为当前检测图，并清理上一来源的结果状态。
        /// </summary>
        /// <param name="image">需要交由主窗体管理的相机图像。</param>
        private void UseCameraImage(HImage image)
        {
            if (image == null) return;

            HalconImageHelper.SafeDispose(ref currentImage);
            currentImage = image;
            ClearRoi();
            DisposeResultImages();
            detectionItems.Clear();
            HalconImageHelper.SetCameraImageMode(SelectedCameraName);
            HalconImageHelper.DisplayImageFitWindow(hWindowAnalysis, currentImage);
            HalconImageHelper.ClearWindow(hWindowMask);
            HalconImageHelper.ClearWindow(hWindowEdge);
            UpdateNavigationState();
            UpdateSourceText();
        }

        /// <summary>
        /// 停止连续采集，并按需检测当前保存的最后一帧图像。
        /// </summary>
        /// <param name="runDetection">停止后是否立即执行缺陷检测。</param>
        private void StopContinuousGrab(bool runDetection)
        {
            if (!isContinuousGrabbing) return;

            isContinuousGrabbing = false;
            HalconCameraHelper.StopGrab();
            UpdateCameraControlState();
            UpdateSourceText();

            if (runDetection && currentImage != null)
                RunDetection();
        }
        #endregion

        #region 图像输入和导航
        /// <summary>
        /// 响应打开图片按钮，加载单张图片、保存来源并执行检测。
        /// </summary>
        /// <param name="sender">触发事件的打开图片按钮。</param>
        /// <param name="e">事件参数。</param>
        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            StopContinuousGrab(false);
            // 单张模式会由 Helper 清空图片列表，导航按钮随即禁用。
            if (!HalconImageHelper.OpenImage(hWindowAnalysis, ref currentImage)) return;
            SaveCurrentImageSource();
            ClearRoi();
            UpdateNavigationState();
            UpdateSourceText();
            RunDetection();
        }

        /// <summary>
        /// 响应打开文件夹按钮，加载文件夹首张图片并执行检测。
        /// </summary>
        /// <param name="sender">触发事件的打开图片文件夹按钮。</param>
        /// <param name="e">事件参数。</param>
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            StopContinuousGrab(false);
            // 文件夹中的图片按名称排序后，从第一张开始检测。
            if (!HalconImageHelper.OpenImageFolder(hWindowAnalysis, ref currentImage)) return;
            SaveCurrentImageSource();
            ClearRoi();
            UpdateNavigationState();
            UpdateSourceText();
            RunDetection();
        }

        /// <summary>
        /// 响应上一张按钮，切换到文件夹中的上一张图片并重新检测。
        /// </summary>
        /// <param name="sender">触发事件的上一张按钮。</param>
        /// <param name="e">事件参数。</param>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (!HalconImageHelper.CanNavigate) return;
            HalconImageHelper.PreviousImage(hWindowAnalysis, ref currentImage);
            ClearRoi();
            UpdateSourceText();
            RunDetection();
        }

        /// <summary>
        /// 响应下一张按钮，切换到文件夹中的下一张图片并重新检测。
        /// </summary>
        /// <param name="sender">触发事件的下一张按钮。</param>
        /// <param name="e">事件参数。</param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!HalconImageHelper.CanNavigate) return;
            HalconImageHelper.NextImage(hWindowAnalysis, ref currentImage);
            ClearRoi();
            UpdateSourceText();
            RunDetection();
        }

        /// <summary>
        /// 根据当前图片列表数量刷新上一张和下一张按钮状态。
        /// </summary>
        private void UpdateNavigationState()
        {
            bool canNavigate = HalconImageHelper.CanNavigate;
            btnPrevious.Enabled = canNavigate;
            btnNext.Enabled = canNavigate;
        }

        /// <summary>
        /// 按当前图像来源和采集状态更新界面来源说明。
        /// </summary>
        private void UpdateSourceText()
        {
            if (HalconImageHelper.CurrentSourceType == HalconImageHelper.CameraSourceType)
            {
                string cameraState = isContinuousGrabbing ? "连续采集预览" : "相机图像";
                lblSource.Text = "当前来源：" + HalconImageHelper.CurrentImagePath +
                    "（" + cameraState + "，不能切换）";
            }
            else if (HalconImageHelper.CurrentSourceType == HalconImageHelper.ImageFolderSourceType)
            {
                lblSource.Text = "当前图片：" + Path.GetFileName(HalconImageHelper.CurrentImagePath) +
                    "（文件夹模式，第 " + (HalconImageHelper.CurrentImageIndex + 1) +
                    " / " + HalconImageHelper.ImageFileCount + " 张）";
            }
            else
            {
                lblSource.Text = "当前图片：" + Path.GetFileName(HalconImageHelper.CurrentImagePath) + "（单张模式，不能切换）";
            }
        }

        /// <summary>
        /// 保存当前单图或文件夹来源，供下次启动时恢复。
        /// </summary>
        private void SaveCurrentImageSource()
        {
            try
            {
                SimpleJsonHelper.ImageSourceInformation information =
                    new SimpleJsonHelper.ImageSourceInformation();
                information.SourceType = HalconImageHelper.CurrentSourceType;
                information.SourcePath = HalconImageHelper.CurrentSourcePath;
                jsonHelper.JsonWriteImageSource(ConfigFolderName, ImageSourceConfigName, information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "保存图片路径失败：" + ex.Message, "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 窗体首次显示时恢复相机配置和最近一次图片来源。
        /// </summary>
        /// <param name="sender">触发事件的主窗体。</param>
        /// <param name="e">事件参数。</param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            // 设备枚举只在用户点击“搜索相机”时执行，启动阶段仅恢复接口和设备配置。
            isLoadingCameraConfig = true;
            LoadCameraConfig();
            isLoadingCameraConfig = false;
            UpdateCameraControlState();
            RestoreLastImageSource();
        }

        /// <summary>
        /// 按保存的来源类型恢复单张图片或图片文件夹。
        /// </summary>
        private void RestoreLastImageSource()
        {
            try
            {
                SimpleJsonHelper.ImageSourceInformation information =
                    jsonHelper.JsonReadImageSource(ConfigFolderName, ImageSourceConfigName);
                if (information == null || string.IsNullOrWhiteSpace(information.SourcePath))
                    return;

                bool loaded = false;
                if (information.SourceType == HalconImageHelper.SingleImageSourceType)
                {
                    if (!File.Exists(information.SourcePath))
                    {
                        lblSource.Text = "上次保存的图片路径已失效，请重新选择。";
                        return;
                    }

                    // 单图恢复后保持单张模式，上一张和下一张按钮继续禁用。
                    loaded = HalconImageHelper.OpenImage(hWindowAnalysis,
                        information.SourcePath, ref currentImage);
                }
                else if (information.SourceType == HalconImageHelper.ImageFolderSourceType)
                {
                    if (!Directory.Exists(information.SourcePath))
                    {
                        lblSource.Text = "上次保存的图片文件夹已失效，请重新选择。";
                        return;
                    }

                    // 文件夹恢复会重新扫描图片列表，并从排序后的第一张开始。
                    loaded = HalconImageHelper.OpenImageFolder(hWindowAnalysis,
                        information.SourcePath, ref currentImage);
                }

                if (!loaded) return;

                ClearRoi();
                UpdateNavigationState();
                UpdateSourceText();
                RunDetection();
            }
            catch (Exception ex)
            {
                lblSource.Text = "读取上次图片路径失败：" + ex.Message;
            }
        }
        #endregion

        #region 检测和显示
        /// <summary>
        /// 响应检测按钮，停止相机预览并对当前图像执行检测。
        /// </summary>
        private void btnDetect_Click(object sender, EventArgs e)
        {
            StopContinuousGrab(false);
            RunDetection();
        }

        /// <summary>
        /// 响应检测参数变化，在已有静态图像时自动重新检测。
        /// </summary>
        private void parameter_ValueChanged(object sender, EventArgs e)
        {
            // 窗体初始化时尚未加载图像，因此仅在有图像时响应用户调参。
            if (currentImage != null && !isContinuousGrabbing)
                RunDetection();
        }

        /// <summary>
        /// 读取界面参数，执行缺陷检测并刷新图像和统计结果。
        /// </summary>
        private void RunDetection()
        {
            if (currentImage == null) return;

            DetectionParameter parameter;
            string parameterMessage;

            //如果读取参数失败，则返回错误信息，并且直接return
            if (!TryReadParameter(out parameter, out parameterMessage))
            {
                lblSummary.Text = "参数错误：" + parameterMessage;
                return;
            }

            HImage newAdjustedImage;
            HImage newMaskImage;
            HImage newEdgeImage;
            List<DetectionItem> newItems;
            string message;

            // 对检测结果进行检查，成功后才替换旧结果，避免处理失败时界面丢失上一帧有效结果。
            if (!DefectDetectionHelper.Detect(currentImage, parameter, roiRegion, out newAdjustedImage,
                out newMaskImage, out newEdgeImage, out newItems, out message))
            {
                MessageBox.Show(this, message, "检测失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DisposeResultImages();
            adjustedImage = newAdjustedImage;
            maskImage = newMaskImage;
            edgeImage = newEdgeImage;
            detectionItems = newItems;
            DisplayDetectionResult();
            UpdateSummary();
        }

        /// <summary>
        /// 从界面读取检测参数，并校验合格面积与实心度范围。
        /// </summary>
        /// <param name="parameter"> 更新后的检测参数对象 </param>
        /// <param name="message"> 如果参数配置的有异常，会附带异常信息 </param>
        /// <returns>参数全部有效时返回 true，否则返回 false。</returns>
        private bool TryReadParameter(out DetectionParameter parameter, out string message)
        {
            parameter = new DetectionParameter();
            message = string.Empty;
            parameter.Brightness = (int)numBrightness.Value;
            parameter.Contrast = (int)numContrast.Value;
            parameter.UseClahe = chkClahe.Checked;
            parameter.BlurLevel = (int)numBlur.Value;
            parameter.ThresholdMode = cmbThresholdMode.SelectedIndex;
            parameter.ThresholdValue = (int)numThreshold.Value;
            parameter.AdaptiveBlock = (int)numAdaptiveBlock.Value;
            parameter.MorphOpen = (int)numMorphOpen.Value;
            parameter.MorphClose = (int)numMorphClose.Value;
            parameter.CannyLow = (int)numCannyLow.Value;
            parameter.QualifiedMinArea = (int)numMinArea.Value;
            parameter.QualifiedMaxArea = (int)numMaxArea.Value;
            parameter.MinSolidityGate = (int)numMinSolidity.Value;
            parameter.MaxSolidityGate = (int)numMaxSolidity.Value;
            parameter.CircularityGate = (int)numCircularity.Value;
            parameter.MaxAspectRatioPercent = (int)numMaxAspectRatio.Value;

            if (parameter.QualifiedMinArea > parameter.QualifiedMaxArea)
            {
                message = "合格最小面积不能大于合格最大面积。";
                return false;
            }

            if (parameter.MinSolidityGate > parameter.MaxSolidityGate)
            {
                message = "最小实心度不能大于最大实心度。";
                return false;
            }

            if (parameter.MaxAspectRatioPercent < 100)
            {
                message = "最大长宽比不能小于 100%。";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 在各 HALCON 窗口显示检测图、ROI、零件标注、Mask 和边缘图。
        /// </summary>
        private void DisplayDetectionResult()
        {
            if (adjustedImage == null) return;

            // 先显示处理后的原图，再绘制 ROI 和每个零件的检测标注。
            HalconImageHelper.DisplayImageFitWindow(hWindowAnalysis, adjustedImage);
            if (roiRegion != null)
                HalconImageHelper.DisplayRegion(hWindowAnalysis, roiRegion, "yellow", "margin");

            foreach (DetectionItem item in detectionItems)
            {
                string color = item.IsDefect ? "red" : "green";
                hWindowAnalysis.HalconWindow.SetColor(color);
                hWindowAnalysis.HalconWindow.SetDraw("margin");
                hWindowAnalysis.HalconWindow.SetLineWidth(2);
                hWindowAnalysis.HalconWindow.DispRectangle1(item.Row1, item.Column1, item.Row2, item.Column2);
                hWindowAnalysis.HalconWindow.DispCross(item.Row, item.Column, 8, 0);

                // 中文字段分行显示，避免完整名称横向遮挡相邻零件。
                string text = (item.IsDefect ? "NG" : "OK") +
                    "  面积：" + item.Area.ToString("F0") + "\n" +
                    "实心度：" + (item.Solidity * 100).ToString("F2") + "%\n" +
                    "圆度：" + (item.Circularity * 100).ToString("F2") + "%";
                hWindowAnalysis.HalconWindow.DispText(text, "image", Math.Max(0, item.Row1 - 56), item.Column1,
                    color, new HTuple(), new HTuple());
            }

            HalconImageHelper.DisplayImageFitWindow(hWindowMask, maskImage);
            HalconImageHelper.DisplayImageFitWindow(hWindowEdge, edgeImage);
        }

        /// <summary>
        /// 响应图像页签切换，主动重绘当前页对应的检测结果。
        /// </summary>
        private void tabImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 未显示的页签可能尚未创建 HALCON 窗口，切换后需要主动重绘当前结果。
            if (tabImage.SelectedTab == tabPageAnalysis && adjustedImage != null)
            {
                DisplayDetectionResult();
            }
            else if (tabImage.SelectedTab == tabPageMask && maskImage != null)
            {
                HalconImageHelper.DisplayImageFitWindow(hWindowMask, maskImage);
            }
            else if (tabImage.SelectedTab == tabPageEdge && edgeImage != null)
            {
                HalconImageHelper.DisplayImageFitWindow(hWindowEdge, edgeImage);
            }
        }

        /// <summary>
        /// 统计 OK、NG 数量及目标覆盖率并更新结果摘要。
        /// </summary>
        private void UpdateSummary()
        {
            int defectCount = 0;
            double areaSum = 0;
            foreach (DetectionItem item in detectionItems)
            {
                areaSum += item.Area;
                if (item.IsDefect) defectCount++;
            }

            // 覆盖率以整张输入图为基准，ROI 只影响参与判定的连通域。
            HTuple width, height;
            HOperatorSet.GetImageSize(currentImage, out width, out height);
            double coverage = areaSum / Math.Max(1, width.D * height.D) * 100.0;
            lblSummary.Text = "OK：" + (detectionItems.Count - defectCount) +
                "    NG：" + defectCount + "    覆盖率：" + coverage.ToString("F2") + "%";
        }
        #endregion

        #region ROI 和报告
        /// <summary>
        /// 响应绘制 ROI 按钮，交互绘制矩形区域并重新检测。
        /// </summary>
        private void btnDrawRoi_Click(object sender, EventArgs e)
        {
            if (currentImage == null)
            {
                MessageBox.Show(this, "请先打开图片。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DisplayDetectionResult();
            double row1, column1, row2, column2;
            if (!HalconVisionHelper.DrawRectangle1(hWindowAnalysis, out row1, out column1, out row2, out column2)) return;
            if (row2 - row1 < 5 || column2 - column1 < 5) return;

            ClearRoi();
            roiRegion = HalconVisionHelper.GenRectangle1(row1, column1, row2, column2);
            RunDetection();
        }

        /// <summary>
        /// 响应重置 ROI 按钮，恢复整幅图像检测。
        /// </summary>
        private void btnResetRoi_Click(object sender, EventArgs e)
        {
            ClearRoi();
            RunDetection();
        }

        /// <summary>
        /// 响应导出按钮，保存标注图、Mask 图和中文检测报告。
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (currentImage == null || adjustedImage == null)
            {
                MessageBox.Show(this, "请先完成一次检测。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string reportFolder = Path.Combine(Application.StartupPath, "reports");
                Directory.CreateDirectory(reportFolder);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string sourceName = Path.GetFileNameWithoutExtension(HalconImageHelper.CurrentImagePath);
                if (string.IsNullOrWhiteSpace(sourceName)) sourceName = "inspection";
                string resultPath = Path.Combine(reportFolder, sourceName + "_" + timestamp + "_result.png");
                string maskPath = Path.Combine(reportFolder, sourceName + "_" + timestamp + "_mask.png");
                string reportPath = Path.Combine(reportFolder, sourceName + "_" + timestamp + "_report.txt");

                // DumpWindowImage 会保存 HALCON 窗口中的文字、矩形和 ROI 标注。
                using (HImage resultImage = hWindowAnalysis.HalconWindow.DumpWindowImage())
                {
                    HOperatorSet.WriteImage(resultImage, "png", 0, resultPath);
                }
                HOperatorSet.WriteImage(maskImage, "png", 0, maskPath);
                File.WriteAllText(reportPath, BuildReportText(), Encoding.UTF8);
                MessageBox.Show(this, "检测结果已导出：\r\n" + reportFolder, "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "导出失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 根据当前参数和零件明细生成中文文本检测报告。
        /// </summary>
        /// <returns>完整的检测报告文本。</returns>
        private string BuildReportText()
        {
            int defectCount = 0;
            StringBuilder builder = new StringBuilder();
            foreach (DetectionItem item in detectionItems)
                if (item.IsDefect) defectCount++;

            builder.AppendLine("VisionDemo2026 零件缺陷检测报告");
            builder.AppendLine("生成时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.AppendLine();
            builder.AppendLine("【检测概况】");
            builder.AppendLine("图像来源：" + HalconImageHelper.CurrentImagePath);
            builder.AppendLine("检测总数：" + detectionItems.Count);
            builder.AppendLine("OK数量：" + (detectionItems.Count - defectCount));
            builder.AppendLine("NG数量：" + defectCount);
            builder.AppendLine();
            builder.AppendLine("【检测参数】");
            builder.AppendLine("阈值模式：" + cmbThresholdMode.Text);
            builder.AppendLine("亮度：" + numBrightness.Value + "，对比度：" + numContrast.Value);
            builder.AppendLine("高斯滤波等级：" + numBlur.Value +
                "，局部对比度增强：" + (chkClahe.Checked ? "启用" : "关闭"));
            builder.AppendLine("手动阈值：" + numThreshold.Value +
                "，开运算半径：" + numMorphOpen.Value + "，闭运算半径：" + numMorphClose.Value);
            builder.AppendLine("Canny 低阈值：" + numCannyLow.Value + "，候选目标面积：1000~8000");
            builder.AppendLine("合格面积范围：" + numMinArea.Value + "~" + numMaxArea.Value);
            builder.AppendLine("合格实心度范围：" + numMinSolidity.Value + "%~" + numMaxSolidity.Value +
                "%，圆度下限：" + numCircularity.Value + "%");
            builder.AppendLine("最大长宽比：" + numMaxAspectRatio.Value + "%（圆环必须保留一个内孔）");
            builder.AppendLine();
            builder.AppendLine("【零件明细】");
            builder.AppendLine("序号,判定结果,中心横坐标,中心纵坐标,面积,周长,圆度(%),实心度(%),填充率(%),内孔数,长宽比,判定原因");
            for (int i = 0; i < detectionItems.Count; i++)
            {
                DetectionItem item = detectionItems[i];
                builder.AppendLine((i + 1) + "," + (item.IsDefect ? "NG" : "OK") + "," +
                    item.Column.ToString("F2") + "," + item.Row.ToString("F2") + "," +
                    item.Area.ToString("F2") + "," + item.Perimeter.ToString("F2") + "," +
                    (item.Circularity * 100).ToString("F2") + "," +
                    (item.Solidity * 100).ToString("F2") + "," +
                    (item.Extent * 100).ToString("F2") + "," + item.HoleCount + "," +
                    item.AspectRatio.ToString("F4") + "," + item.DefectReason);
            }
            return builder.ToString();
        }
        #endregion

        #region 资源释放
        /// <summary>
        /// 释放并清空当前 ROI 区域。
        /// </summary>
        private void ClearRoi()
        {
            if (roiRegion != null)
            {
                roiRegion.Dispose();
                roiRegion = null;
            }
        }

        /// <summary>
        /// 释放处理图、Mask 图和边缘图等检测结果资源。
        /// </summary>
        private void DisposeResultImages()
        {
            HalconImageHelper.SafeDispose(ref adjustedImage);
            HalconImageHelper.SafeDispose(ref maskImage);
            HalconImageHelper.SafeDispose(ref edgeImage);
        }

        /// <summary>
        /// 窗体关闭时停止相机并释放全部 HALCON 资源。
        /// </summary>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 统一释放 HALCON 图像和区域句柄，避免窗体重复打开后占用资源。
            isContinuousGrabbing = false;
            HalconCameraHelper.StopGrab();
            try
            {
                HalconCameraHelper.Close(ref cameraHandle);
            }
            catch
            {
                // 相机已断开时关闭句柄可能失败，继续释放其余图像资源。
            }
            ClearRoi();
            DisposeResultImages();
            HalconImageHelper.SafeDispose(ref currentImage);
            HalconImageHelper.ClearWindow(hWindowAnalysis);
            HalconImageHelper.ClearWindow(hWindowMask);
            HalconImageHelper.ClearWindow(hWindowEdge);
        }
        #endregion

    }
}
