using System;
using System.Collections.Generic;
using HalconDotNet;

namespace VisionDemo2026
{
    public class DetectionParameter
    {
        public int Brightness = 50;
        public int Contrast = 50;
        public bool UseClahe = true;
        public int BlurLevel = 1;
        public int ThresholdMode = 1;
        public int ThresholdValue = 115;
        public int AdaptiveBlock = 7;
        public int MorphOpen = 1;
        public int MorphClose = 2;
        public int CannyLow = 70;
        public int QualifiedMinArea = 4000;
        public int QualifiedMaxArea = 5500;
        public int MinSolidityGate = 80;
        public int MaxSolidityGate = 90;
        public int CircularityGate = 45;
    }

    public class DetectionItem
    {
        public double Row1;
        public double Column1;
        public double Row2;
        public double Column2;
        public double Row;
        public double Column;
        public double Area;
        public double Perimeter;
        public double Circularity;
        public double Solidity;
        public double Extent;
        public double AspectRatio;
        public bool IsDefect;
    }

    public class DefectDetectionHelper
    {
        // 候选区域只负责排除明显噪声和大面积背景，不受界面合格参数影响。
        private const int DetectionMinArea = 1000;
        private const int DetectionMaxArea = 8000;

        #region 零件缺陷检测
        /// <summary>
        /// 对输入图像执行预处理、分割、区域筛选和几何特征判定。
        /// </summary>
        /// <param name="sourceImage">待检测的原始图像。</param>
        /// <param name="parameter">界面配置的检测与合格判定参数。</param>
        /// <param name="roiRegion">限制检测范围的 ROI，为 null 时检测整图。</param>
        /// <param name="adjustedImage">输出亮度、对比度调整后的显示图像。</param>
        /// <param name="maskImage">输出目标区域的二值 Mask 图像。</param>
        /// <param name="edgeImage">输出经过灰度拉伸的 Canny 边缘图像。</param>
        /// <param name="items">输出每个候选零件的特征及判定结果。</param>
        /// <param name="message">输出检测结果或失败原因。</param>
        /// <returns>检测流程成功完成时返回 true，否则返回 false。</returns>
        public static bool Detect(HImage sourceImage, DetectionParameter parameter,
                                  HObject roiRegion, out HImage adjustedImage,
                                  out HImage maskImage, out HImage edgeImage,
                                  out List<DetectionItem> items, out string message)
        {
            adjustedImage = null;
            maskImage = null;
            edgeImage = null;
            items = new List<DetectionItem>();
            message = string.Empty;

            if (sourceImage == null)
            {
                message = "请先打开检测图片。";
                return false;
            }

            HImage grayImage = null;
            HImage enhancedImage = null;
            HImage filteredImage = null;
            HImage backgroundImage = null;
            HObject thresholdRegion = null;
            HObject openedRegion = null;
            HObject closedRegion = null;
            HObject activeRegion = null;
            HObject connectedRegions = null;
            HObject selectedRegion = null;
            HObject edgeAmplitude = null;
            HObject edgeDirection = null;
            HImage edgeAmplitudeImage = null;

            try
            {
                double alpha = Math.Max(0.2, parameter.Contrast / 50.0);
                int beta = parameter.Brightness - 50;
                adjustedImage = HalconImageProcessHelper.ScaleImage(sourceImage, alpha, beta);
                grayImage = GetGrayImage(adjustedImage);

                if (parameter.UseClahe)
                {
                    HObject localEqualized;
                    HOperatorSet.EquHistoImageRect(grayImage, out localEqualized,
                        "fast", 31, 31, 0.02);
                    enhancedImage = new HImage(localEqualized);
                }
                else
                {
                    enhancedImage = grayImage.CopyImage();
                }

                int blurSize = GetOddKernelSize(parameter.BlurLevel);
                filteredImage = parameter.BlurLevel > 0
                    ? HalconImageProcessHelper.GaussImage(enhancedImage, blurSize)
                    : enhancedImage.CopyImage();

                thresholdRegion = CreateThresholdRegion(filteredImage, parameter, out backgroundImage);
                if (thresholdRegion == null)
                {
                    message = "阈值分割失败。";
                    return false;
                }

                openedRegion = parameter.MorphOpen > 0
                    ? HalconRegionHelper.OpeningCircle(thresholdRegion, parameter.MorphOpen)
                    : thresholdRegion.CopyObj(1, -1);
                closedRegion = parameter.MorphClose > 0
                    ? HalconRegionHelper.ClosingCircle(openedRegion, parameter.MorphClose)
                    : openedRegion.CopyObj(1, -1);
                maskImage = CreateMaskImage(closedRegion, filteredImage);

                HOperatorSet.EdgesImage(filteredImage, out edgeAmplitude, out edgeDirection,
                    "canny", 1, "nms", parameter.CannyLow,
                    Math.Min(255, Math.Max(parameter.CannyLow + 1, parameter.CannyLow * 2)));
                // Canny 幅值通常集中在较低灰度，拉伸到 0~255 后边缘才能清晰显示。
                edgeAmplitudeImage = new HImage(edgeAmplitude);
                edgeImage = edgeAmplitudeImage.ScaleImageMax();

                activeRegion = closedRegion.CopyObj(1, -1);
                if (roiRegion != null)
                {
                    HObject roiResult;
                    HOperatorSet.Intersection(activeRegion, roiRegion, out roiResult);
                    activeRegion.Dispose();
                    activeRegion = roiResult;
                }

                connectedRegions = HalconRegionHelper.Connection(activeRegion);
                selectedRegion = HalconRegionHelper.SelectShape(connectedRegions, "area", "and",
                    DetectionMinArea, DetectionMaxArea);
                ReadDetectionItems(selectedRegion, parameter, items);
                message = "检测完成，共发现 " + items.Count + " 个零件。";
                return true;
            }
            catch (Exception ex)
            {
                SafeDispose(ref adjustedImage);
                SafeDispose(ref maskImage);
                SafeDispose(ref edgeImage);
                message = "检测失败: " + ex.Message;
                return false;
            }
            finally
            {
                SafeDispose(ref grayImage);
                SafeDispose(ref enhancedImage);
                SafeDispose(ref filteredImage);
                SafeDispose(ref backgroundImage);
                SafeDispose(ref thresholdRegion);
                SafeDispose(ref openedRegion);
                SafeDispose(ref closedRegion);
                SafeDispose(ref activeRegion);
                SafeDispose(ref connectedRegions);
                SafeDispose(ref selectedRegion);
                SafeDispose(ref edgeAmplitudeImage);
                SafeDispose(ref edgeAmplitude);
                SafeDispose(ref edgeDirection);
            }
        }
        #endregion

        #region 私有检测方法
        /// <summary>
        /// 将输入图像转换为单通道灰度图，灰度图输入则直接复制。
        /// </summary>
        /// <param name="image">待转换的 HALCON 图像。</param>
        /// <returns>独立持有资源的灰度图像。</returns>
        private static HImage GetGrayImage(HImage image)
        {
            HTuple channelCount;
            HOperatorSet.CountChannels(image, out channelCount);
            return channelCount.I == 1 ? image.CopyImage() : HalconImageHelper.RgbToGray(image);
        }

        /// <summary>
        /// 根据阈值模式创建目标区域，并在动态阈值模式下输出背景图。
        /// </summary>
        /// <param name="image">用于阈值分割的灰度图像。</param>
        /// <param name="parameter">包含阈值模式和相关数值的检测参数。</param>
        /// <param name="backgroundImage">输出动态阈值使用的背景图，其他模式为 null。</param>
        /// <returns>阈值分割得到的目标区域。</returns>
        private static HObject CreateThresholdRegion(HImage image, DetectionParameter parameter,
                                                     out HImage backgroundImage)
        {
            backgroundImage = null;
            if (parameter.ThresholdMode == 0)
                return HalconRegionHelper.Threshold(image, 0, parameter.ThresholdValue);

            if (parameter.ThresholdMode == 1)
            {
                HObject result;
                HTuple usedThreshold;
                HOperatorSet.BinaryThreshold(image, out result, "max_separability", "dark", out usedThreshold);
                return result;
            }

            // 动态阈值的背景窗口必须明显大于零件直径，避免只提取到零件边缘。
            int blockSize = Math.Max(51, GetOddKernelSize(parameter.AdaptiveBlock));
            backgroundImage = HalconImageProcessHelper.MeanImage(image, blockSize, blockSize);
            HObject region;
            HOperatorSet.DynThreshold(image, backgroundImage, out region, 5, "dark");
            return region;
        }

        /// <summary>
        /// 按参考图像尺寸将目标区域绘制为 0/255 二值 Mask 图。
        /// </summary>
        /// <param name="region">需要绘制的目标区域。</param>
        /// <param name="referenceImage">用于确定图像尺寸和域的参考图像。</param>
        /// <returns>目标区域为白色的二值图像。</returns>
        private static HImage CreateMaskImage(HObject region, HImage referenceImage)
        {
            HObject image;
            HOperatorSet.GenImageProto(referenceImage, out image, 0);
            HObject paintedImage;
            HOperatorSet.PaintRegion(region, image, out paintedImage, 255, "fill");
            image.Dispose();
            return new HImage(paintedImage);
        }

        /// <summary>
        /// 提取候选区域的几何特征，并按合格范围生成零件判定结果。
        /// </summary>
        /// <param name="regions">已经过候选面积筛选的连通区域集合。</param>
        /// <param name="parameter">面积、实心度和圆度合格判定参数。</param>
        /// <param name="items">接收零件检测结果的列表。</param>
        private static void ReadDetectionItems(HObject regions, DetectionParameter parameter,
                                               List<DetectionItem> items)
        {
            if (regions == null) return;

            HTuple count;
            HOperatorSet.CountObj(regions, out count);
            for (int i = 1; i <= count.I; i++)
            {
                HObject region;
                HOperatorSet.SelectObj(regions, out region, i);
                HObject contour = null;
                try
                {
                    HTuple area, row, column;
                    HTuple row1, column1, row2, column2;
                    HTuple circularity, convexity;
                    HOperatorSet.AreaCenter(region, out area, out row, out column);
                    HOperatorSet.SmallestRectangle1(region, out row1, out column1, out row2, out column2);
                    HOperatorSet.Circularity(region, out circularity);
                    HOperatorSet.Convexity(region, out convexity);
                    HOperatorSet.GenContourRegionXld(region, out contour, "border");
                    HTuple perimeter;
                    HOperatorSet.LengthXld(contour, out perimeter);

                    double width = Math.Max(1, column2.D - column1.D + 1);
                    double height = Math.Max(1, row2.D - row1.D + 1);
                    DetectionItem item = new DetectionItem();
                    item.Row1 = row1.D;
                    item.Column1 = column1.D;
                    item.Row2 = row2.D;
                    item.Column2 = column2.D;
                    item.Row = row.D;
                    item.Column = column.D;
                    item.Area = area.D;
                    item.Perimeter = perimeter.D;
                    item.Circularity = circularity.D;
                    item.Solidity = convexity.D;
                    item.Extent = area.D / (width * height);
                    item.AspectRatio = width / height;
                    bool areaQualified = item.Area >= parameter.QualifiedMinArea &&
                                         item.Area <= parameter.QualifiedMaxArea;
                    bool solidityQualified = item.Solidity >= parameter.MinSolidityGate / 100.0 &&
                                             item.Solidity <= parameter.MaxSolidityGate / 100.0;
                    bool circularityQualified = item.Circularity >= parameter.CircularityGate / 100.0;

                    // 面积、实心度或圆度任一指标不合格，该零件即判定为 NG。
                    item.IsDefect = !areaQualified || !solidityQualified || !circularityQualified;
                    items.Add(item);
                }
                finally
                {
                    SafeDispose(ref contour);
                    SafeDispose(ref region);
                }
            }
        }

        /// <summary>
        /// 将界面等级换算为不小于 1 的奇数滤波核尺寸。
        /// </summary>
        /// <param name="level">界面设置的滤波或背景窗口等级。</param>
        /// <returns>可供 HALCON 算子使用的奇数核尺寸。</returns>
        private static int GetOddKernelSize(int level)
        {
            return Math.Max(1, level * 2 + 1);
        }

        /// <summary>
        /// 安全释放 HALCON 图像并将引用置空。
        /// </summary>
        /// <param name="image">需要释放的图像引用。</param>
        private static void SafeDispose(ref HImage image)
        {
            if (image != null)
            {
                image.Dispose();
                image = null;
            }
        }

        /// <summary>
        /// 安全释放 HALCON 图标对象并将引用置空。
        /// </summary>
        /// <param name="obj">需要释放的 HALCON 对象引用。</param>
        private static void SafeDispose(ref HObject obj)
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = null;
            }
        }
        #endregion
    }
}
