using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HalconDotNet;

namespace VisionDemo2026
{
    public class HalconImageHelper
    {
        public const string SingleImageSourceType = "SingleImage";
        public const string ImageFolderSourceType = "ImageFolder";
        public const string CameraSourceType = "Camera";

        SimpleJsonHelper _jsonHelper =new SimpleJsonHelper();

        #region 选择图片并加载
        /// <summary>
        /// 弹出单选图片对话框，加载图像并显示到指定窗口。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="loadedImage">输出加载成功的图像，由调用方负责释放。</param>
        /// <returns>用户选择且图像加载成功时返回 true。</returns>
        public static bool LoadLocalImage(HWindowControl window, out HImage loadedImage)
        {
            loadedImage = null;

            if (window == null)
            {
                ShowError("窗口控件为空");
                return false;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = GetImageFilter();
            ofd.Title = "选择图像";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK)
                return false;

            return LoadLocalImage(window, ofd.FileName, out loadedImage);
        }
        #endregion

        #region 打开图片（自动释放旧图）
        /// <summary>
        /// 弹出单选图片对话框，并用新图像替换当前图像。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="image">当前图像引用，成功后替换并释放旧图像。</param>
        /// <returns>用户选择且图像打开成功时返回 true。</returns>
        public static bool OpenImage(HWindowControl window, ref HImage image)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = GetImageFilter();
                openFileDialog.Title = "选择图像";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return false;

                return OpenImage(window, openFileDialog.FileName, ref image);
            }
        }

        /// <summary>
        /// 从指定路径打开单张图片，并切换为单图来源模式。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="filePath">待打开图片的完整路径。</param>
        /// <param name="image">当前图像引用，成功后替换并释放旧图像。</param>
        /// <returns>图片打开成功时返回 true。</returns>
        public static bool OpenImage(HWindowControl window, string filePath, ref HImage image)
        {
            HImage loadedImage;
            if (!LoadLocalImage(window, filePath, out loadedImage))
                return false;

            SafeDispose(ref image);
            image = loadedImage;
            SetSingleImageMode();
            currentSourceType = SingleImageSourceType;
            currentSourcePath = Path.GetFullPath(filePath);
            return true;
        }
        #endregion

        #region 加载图片（指定路径）
        /// <summary>
        /// 从指定路径加载图像并按比例显示，但不替换调用方已有图像。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="filePath">待加载图片的完整路径。</param>
        /// <param name="loadedImage">输出新加载的图像，由调用方负责释放。</param>
        /// <returns>图像加载成功时返回 true。</returns>
        public static bool LoadLocalImage(HWindowControl window, string filePath, out HImage loadedImage)
        {
            loadedImage = null;

            if (window == null) { ShowError("窗口控件为空"); return false; }
            if (string.IsNullOrEmpty(filePath)) { ShowError("文件路径为空"); return false; }
            if (!File.Exists(filePath)) { ShowError("文件不存在: " + filePath); return false; }

            try
            {
                loadedImage = new HImage(filePath);
                DisplayImageFitWindow(window, loadedImage);
                currentImagePath = filePath;
                return true;
            }
            catch (Exception ex)
            {
                ShowError("加载失败: " + ex.Message);
                if (loadedImage != null) { loadedImage.Dispose(); loadedImage = null; }
                return false;
            }
        }
        #endregion
        static List<string> imageFiles = new List<string>();
        static int currentImageIndex = -1;
        static string currentImagePath = string.Empty;
        static string currentSourceType = string.Empty;
        static string currentSourcePath = string.Empty;

        public static bool CanNavigate
        {
            get { return imageFiles.Count > 1; }
        }

        public static int CurrentImageIndex
        {
            get { return currentImageIndex; }
        }

        public static int ImageFileCount
        {
            get { return imageFiles.Count; }
        }

        public static string CurrentImagePath
        {
            get { return currentImagePath; }
        }

        public static string CurrentSourceType
        {
            get { return currentSourceType; }
        }

        public static string CurrentSourcePath
        {
            get { return currentSourcePath; }
        }

        #region 打开图片文件夹
        /// <summary>
        /// 弹出文件夹选择对话框，并加载所选文件夹的第一张图片。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="currentImage">当前图像引用，成功后替换并释放旧图像。</param>
        /// <returns>文件夹中存在支持的图片且首张加载成功时返回 true。</returns>
        public static bool OpenImageFolder(HWindowControl window, ref HImage currentImage)
        {
            if (window == null) { ShowError("窗口控件为空"); return false; }

            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "请选择图片文件夹";                   // 设置描述
                folderBrowserDialog.ShowNewFolderButton = false;                     // 不显示新建文件夹

                if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                    return false;

                return OpenImageFolder(window, folderBrowserDialog.SelectedPath, ref currentImage);
            }
        }

        /// <summary>
        /// 扫描指定文件夹中的图片，按名称排序并加载第一张。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="folderPath">待扫描的图片文件夹路径。</param>
        /// <param name="currentImage">当前图像引用，成功后替换并释放旧图像。</param>
        /// <returns>文件夹图像列表建立且首张加载成功时返回 true。</returns>
        public static bool OpenImageFolder(HWindowControl window, string folderPath, ref HImage currentImage)
        {
            if (window == null) { ShowError("窗口控件为空"); return false; }
            if (string.IsNullOrWhiteSpace(folderPath)) { ShowError("文件夹路径为空"); return false; }
            if (!Directory.Exists(folderPath)) { ShowError("文件夹不存在: " + folderPath); return false; }

            List<string> selectedFiles = GetImageFiles(folderPath);
            if (selectedFiles.Count == 0)
            {
                ShowError("文件夹中没有找到支持的图片文件");
                return false;
            }

            imageFiles = selectedFiles;
            currentImageIndex = 0;
            if (!ReplaceCurrentImage(window, imageFiles[currentImageIndex], ref currentImage))
            {
                imageFiles.Clear();
                currentImageIndex = -1;
                return false;
            }

            currentSourceType = ImageFolderSourceType;
            currentSourcePath = Path.GetFullPath(folderPath);
            return true;
        }
        #endregion

        #region 上一张图片
        /// <summary>
        /// 循环切换到图片列表中的上一张图像。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="currentImage">当前图像引用，成功后替换并释放旧图像。</param>
        public static void PreviousImage(HWindowControl window, ref HImage currentImage)
        {
            if (window == null) { ShowError("窗口控件为空"); return; }

            if (imageFiles.Count <= 0)
            {
                ShowError("没有图片文件");
                return;
            }

            if (currentImageIndex <= 0)
            {
                currentImageIndex = imageFiles.Count - 1;
            }
            else
            {
                currentImageIndex--;
            }

            ReplaceCurrentImage(window, imageFiles[currentImageIndex], ref currentImage);
        }

        #endregion
        #region 下一张图片
        /// <summary>
        /// 循环切换到图片列表中的下一张图像。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="currentImage">当前图像引用，成功后替换并释放旧图像。</param>
        public static void NextImage(HWindowControl window, ref HImage currentImage)
        {
            if (window == null) { ShowError("窗口控件为空"); return; }

            if (imageFiles.Count <= 0)
            {
                ShowError("没有图片文件");
                return;
            }

            if (currentImageIndex < 0)
            {
                currentImageIndex = 0;
            }
            else
            {
                currentImageIndex++;
            }

            if (currentImageIndex >= imageFiles.Count)
            {
                currentImageIndex = 0;
            }

            ReplaceCurrentImage(window, imageFiles[currentImageIndex], ref currentImage);
        }
        #endregion

        #region 显示图像（原图1:1像素）
        /// <summary>
        /// 按原始像素比例显示图像，并保存窗口显示图像的副本。
        /// </summary>
        /// <param name="window">目标 HALCON 窗口控件。</param>
        /// <param name="image">需要显示的图像。</param>
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
        /// 保持图像宽高比缩放显示，并用窗口留白补齐多余区域。
        /// </summary>
        /// <param name="window">目标 HALCON 窗口控件。</param>
        /// <param name="image">需要显示的图像。</param>
        public static void DisplayImageFitWindow(HWindowControl window, HImage image)
        {
            if (window == null || image == null) return;

            HTuple width, height;
            HOperatorSet.GetImageSize(image, out width, out height);
            int imageWidth = width.I;
            int imageHeight = height.I;
            int windowWidth = Math.Max(1, window.ClientSize.Width);
            int windowHeight = Math.Max(1, window.ClientSize.Height);

            // 根据窗口宽高比扩展显示区域，使用黑边补齐，避免图像被横向或纵向拉伸。
            double imageRatio = imageWidth / (double)imageHeight;
            double windowRatio = windowWidth / (double)windowHeight;
            double row1 = 0;
            double column1 = 0;
            double row2 = imageHeight - 1;
            double column2 = imageWidth - 1;

            if (windowRatio > imageRatio)
            {
                double displayWidth = imageHeight * windowRatio;
                double columnPadding = (displayWidth - imageWidth) / 2.0;
                column1 = -columnPadding;
                column2 = imageWidth - 1 + columnPadding;
            }
            else
            {
                double displayHeight = imageWidth / windowRatio;
                double rowPadding = (displayHeight - imageHeight) / 2.0;
                row1 = -rowPadding;
                row2 = imageHeight - 1 + rowPadding;
            }

            window.HalconWindow.SetPart(row1, column1, row2, column2);
            window.HalconWindow.ClearWindow();
            window.HalconWindow.DispImage(image);

            HImage oldImage = window.Tag as HImage;
            if (oldImage != null)
                oldImage.Dispose();

            window.Tag = image.CopyImage();
        }
        #endregion

        #region 显示区域
        /// <summary>
        /// 使用指定颜色和绘制模式在窗口中叠加显示区域。
        /// </summary>
        /// <param name="window">目标 HALCON 窗口控件。</param>
        /// <param name="region">需要显示的区域。</param>
        /// <param name="color">HALCON 支持的显示颜色名称。</param>
        /// <param name="draw">绘制模式，例如 fill 或 margin。</param>
        public static void DisplayRegion(HWindowControl window, HObject region, string color, string draw)
        {
            if (window == null || region == null) return;
            window.HalconWindow.SetColor(color);
            window.HalconWindow.SetDraw(draw);
            window.HalconWindow.DispObj(region);
        }
        #endregion

        #region 清空窗口
        /// <summary>
        /// 清空 HALCON 窗口并释放保存在 Tag 中的图像副本。
        /// </summary>
        /// <param name="window">需要清空的 HALCON 窗口控件。</param>
        public static void ClearWindow(HWindowControl window)
        {
            if (window == null) return;
            window.HalconWindow.ClearWindow();

            HImage image = window.Tag as HImage;
            if (image != null)
                image.Dispose();

            window.Tag = null;
        }
        #endregion

        #region RGB 转灰度
        /// <summary>
        /// 将 RGB 彩色图像转换为单通道灰度图像。
        /// </summary>
        /// <param name="colorImage">待转换的彩色图像。</param>
        /// <returns>转换后的灰度图像，输入为空时返回 null。</returns>
        public static HImage RgbToGray(HImage colorImage)
        {
            if (colorImage == null) return null;
            HObject gray;
            HOperatorSet.Rgb1ToGray(colorImage, out gray);
            return new HImage(gray);
        }
        #endregion

        #region 图像缩放
        /// <summary>
        /// 按相同比例缩放图像宽度和高度。
        /// </summary>
        /// <param name="image">待缩放的图像。</param>
        /// <param name="scale">水平和垂直方向的缩放倍数。</param>
        /// <returns>缩放后的新图像。</returns>
        public static HImage ZoomImage(HImage image, double scale)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.ZoomImageFactor(image, out result, scale, scale, "constant");
            return new HImage(result);
        }
        #endregion

        #region 图像旋转
        /// <summary>
        /// 按指定角度和插值方式旋转图像。
        /// </summary>
        /// <param name="image">待旋转的图像。</param>
        /// <param name="angleRad">旋转角度，单位为弧度。</param>
        /// <param name="interpolation">HALCON 插值方式。</param>
        /// <returns>旋转后的新图像。</returns>
        public static HImage RotateImage(HImage image, double angleRad, string interpolation)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.RotateImage(image, out result, angleRad, interpolation);
            return new HImage(result);
        }
        #endregion

        #region 图像镜像
        /// <summary>
        /// 按指定方向对图像进行镜像。
        /// </summary>
        /// <param name="image">待镜像的图像。</param>
        /// <param name="mode">HALCON 镜像模式，例如 row 或 column。</param>
        /// <returns>镜像后的新图像。</returns>
        public static HImage MirrorImage(HImage image, string mode)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.MirrorImage(image, out result, mode);
            return new HImage(result);
        }
        #endregion

        #region 裁剪图像
        /// <summary>
        /// 按矩形坐标裁剪图像。
        /// </summary>
        /// <param name="image">待裁剪的图像。</param>
        /// <param name="row1">裁剪区域左上角行坐标。</param>
        /// <param name="col1">裁剪区域左上角列坐标。</param>
        /// <param name="row2">裁剪区域右下角行坐标。</param>
        /// <param name="col2">裁剪区域右下角列坐标。</param>
        /// <returns>裁剪后的新图像。</returns>
        public static HImage CropRectangle(HImage image, double row1, double col1, double row2, double col2)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.CropRectangle1(image, out result, row1, col1, row2, col2);
            return new HImage(result);
        }
        #endregion

        #region 保存图片（弹窗）
        /// <summary>
        /// 弹出保存对话框，将窗口当前保存的图像副本写入文件。
        /// </summary>
        /// <param name="hWindowControl">包含待保存图像的 HALCON 窗口控件。</param>
        /// <returns>用户确认且图像保存成功时返回 true。</returns>
        public static bool SaveImageWithDialog(HWindowControl hWindowControl)
        {
            if (hWindowControl == null)
            {
                ShowError("窗口控件为空");
                return false;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "BMP|*.bmp|JPEG|*.jpg|PNG|*.png|TIFF|*.tif|所有文件|*.*";
            sfd.Title = "保存图像";
            sfd.DefaultExt = "bmp";
            sfd.AddExtension = true;

            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            return SaveImage(hWindowControl, sfd.FileName);
        }
        #endregion

        #region 保存图片（指定路径）
        /// <summary>
        /// 将 HALCON 窗口 Tag 中的图像按文件扩展名保存到指定路径。
        /// </summary>
        /// <param name="hWindowControl">包含待保存图像的 HALCON 窗口控件。</param>
        /// <param name="filePath">图像输出文件的完整路径。</param>
        /// <returns>图像写入成功时返回 true。</returns>
        public static bool SaveImage(HWindowControl hWindowControl, string filePath)
        {
            if (hWindowControl == null) { ShowError("窗口控件为空"); return false; }
            if (string.IsNullOrEmpty(filePath)) { ShowError("保存路径为空"); return false; }

            HImage image = hWindowControl.Tag as HImage;
            if (image == null) { ShowError("没有可保存的图像"); return false; }

            try
            {
                string ext = Path.GetExtension(filePath).ToLower().TrimStart('.');
                if (ext == "jpeg") ext = "jpg";
                HOperatorSet.WriteImage(image, ext, 0, filePath);
                return true;
            }
            catch (Exception ex)
            {
                ShowError("保存失败: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region 图像相加
        /// <summary>
        /// 对两幅图像执行带乘数和偏移量的逐像素相加。
        /// </summary>
        /// <param name="image1">第一幅输入图像。</param>
        /// <param name="image2">第二幅输入图像。</param>
        /// <param name="mult">相加结果的乘数。</param>
        /// <param name="add">相加结果的灰度偏移量。</param>
        /// <returns>图像相加后的新图像。</returns>
        public static HImage AddImage(HImage image1, HImage image2, double mult, double add)
        {
            if (image1 == null || image2 == null) return null;
            HObject result;
            HOperatorSet.AddImage(image1, image2, out result, mult, add);
            return new HImage(result);
        }
        #endregion

        #region 图像相减
        /// <summary>
        /// 对两幅图像执行带乘数和偏移量的逐像素相减。
        /// </summary>
        /// <param name="image1">被减图像。</param>
        /// <param name="image2">减数图像。</param>
        /// <param name="mult">相减结果的乘数。</param>
        /// <param name="add">相减结果的灰度偏移量。</param>
        /// <returns>图像相减后的新图像。</returns>
        public static HImage SubImage(HImage image1, HImage image2, double mult, double add)
        {
            if (image1 == null || image2 == null) return null;
            HObject result;
            HOperatorSet.SubImage(image1, image2, out result, mult, add);
            return new HImage(result);
        }
        #endregion

        /// <summary>
        /// 加载指定图片并安全替换当前图像资源。
        /// </summary>
        /// <param name="window">用于显示图像的 HALCON 窗口控件。</param>
        /// <param name="filePath">待加载图片的完整路径。</param>
        /// <param name="currentImage">当前图像引用，成功后替换并释放旧图像。</param>
        /// <returns>新图像加载并替换成功时返回 true。</returns>
        private static bool ReplaceCurrentImage(HWindowControl window, string filePath, ref HImage currentImage)
        {
            if (!LoadLocalImage(window, filePath, out HImage loadedImage))
                return false;

            SafeDispose(ref currentImage);
            currentImage = loadedImage;
            currentImagePath = filePath;
            return true;
        }

        #region 单张图片模式
        /// <summary>
        /// 清空文件夹图片列表并切换到不可导航的单图状态。
        /// </summary>
        public static void SetSingleImageMode()
        {
            imageFiles.Clear();
            currentImageIndex = -1;
        }
        #endregion

        #region 相机图像模式
        /// <summary>
        /// 清空文件图片状态并将当前来源标记为相机。
        /// </summary>
        /// <param name="cameraName">用于界面显示的相机名称。</param>
        public static void SetCameraImageMode(string cameraName)
        {
            imageFiles.Clear();
            currentImageIndex = -1;
            currentSourceType = CameraSourceType;
            currentSourcePath = string.Empty;
            currentImagePath = string.IsNullOrWhiteSpace(cameraName)
                ? "相机图像"
                : cameraName.Trim();
        }
        #endregion

        #region 释放图像
        /// <summary>
        /// 安全释放 HALCON 图像并将引用置空。
        /// </summary>
        /// <param name="image">需要释放的图像引用。</param>
        public static void SafeDispose(ref HImage image)
        {
            if (image != null)
            {
                image.Dispose();
                image = null;
            }
        }
        #endregion

        #region 释放对象
        /// <summary>
        /// 安全释放 HALCON 图标对象并将引用置空。
        /// </summary>
        /// <param name="obj">需要释放的 HALCON 对象引用。</param>
        public static void SafeDispose(ref HObject obj)
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = null;
            }
        }
        #endregion

        #region 私有辅助
        /// <summary>
        /// 使用统一标题弹出图像处理错误信息。
        /// </summary>
        /// <param name="message">需要向用户显示的错误内容。</param>
        private static void ShowError(string message)
        {
            MessageBox.Show(message, "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 获取打开图片对话框使用的文件类型筛选字符串。
        /// </summary>
        /// <returns>支持的图片扩展名筛选字符串。</returns>
        private static string GetImageFilter()
        {
            return "所有图像|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.gif;*.jp2;*.pcx;*.pgm;*.ppm;*.pbm;*.xwd;*.ima"
                 + "|BMP|*.bmp"
                 + "|JPEG|*.jpg;*.jpeg"
                 + "|PNG|*.png"
                 + "|TIFF|*.tif;*.tiff"
                 + "|所有文件|*.*";
        }

        /// <summary>
        /// 扫描文件夹顶层的支持图片，并按文件名排序。
        /// </summary>
        /// <param name="folderPath">需要扫描的文件夹路径。</param>
        /// <returns>排序后的图片完整路径列表。</returns>
        private static List<string> GetImageFiles(string folderPath)
        {
            List<string> selectedFiles = new List<string>();
            string[] filters = { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.tif", "*.tiff" };

            foreach (string filter in filters)
                selectedFiles.AddRange(Directory.GetFiles(folderPath, filter, SearchOption.TopDirectoryOnly));

            selectedFiles.Sort(StringComparer.OrdinalIgnoreCase);
            return selectedFiles;
        }
        #endregion


        

    }
}
