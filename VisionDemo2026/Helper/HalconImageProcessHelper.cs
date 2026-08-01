using System;
using HalconDotNet;

namespace VisionDemo2026
{
    public class HalconImageProcessHelper
    {
        #region 灰度缩放
        /// <summary>
        /// 按乘数和偏移量线性调整图像灰度。
        /// </summary>
        /// <param name="image">待调整的输入图像。</param>
        /// <param name="mult">灰度乘数。</param>
        /// <param name="add">灰度偏移量。</param>
        /// <returns>调整后的新图像，输入为空时返回 null。</returns>
        public static HImage ScaleImage(HImage image, double mult, double add)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.ScaleImage(image, out result, mult, add);
            return new HImage(result);
        }
        #endregion

        #region 图像增强
        /// <summary>
        /// 使用指定掩模和增强系数突出图像细节。
        /// </summary>
        /// <param name="image">待增强的输入图像。</param>
        /// <param name="maskWidth">增强掩模宽度。</param>
        /// <param name="maskHeight">增强掩模高度。</param>
        /// <param name="factor">细节增强系数。</param>
        /// <returns>增强后的新图像，输入为空时返回 null。</returns>
        public static HImage Emphasize(HImage image, int maskWidth, int maskHeight, double factor)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.Emphasize(image, out result, maskWidth, maskHeight, factor);
            return new HImage(result);
        }
        #endregion

        #region 均值滤波
        /// <summary>
        /// 使用矩形掩模对图像执行均值滤波。
        /// </summary>
        /// <param name="image">待滤波的输入图像。</param>
        /// <param name="maskWidth">均值掩模宽度。</param>
        /// <param name="maskHeight">均值掩模高度。</param>
        /// <returns>均值滤波后的新图像，输入为空时返回 null。</returns>
        public static HImage MeanImage(HImage image, int maskWidth, int maskHeight)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.MeanImage(image, out result, maskWidth, maskHeight);
            return new HImage(result);
        }
        #endregion

        #region 中值滤波
        /// <summary>
        /// 使用指定掩模类型和半径对图像执行中值滤波。
        /// </summary>
        /// <param name="image">待滤波的输入图像。</param>
        /// <param name="maskType">HALCON 中值滤波掩模类型。</param>
        /// <param name="radius">滤波掩模半径。</param>
        /// <returns>中值滤波后的新图像，输入为空时返回 null。</returns>
        public static HImage MedianImage(HImage image, string maskType, int radius)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.MedianImage(image, out result, maskType, radius, "mirrored");
            return new HImage(result);
        }
        #endregion

        #region 高斯滤波
        /// <summary>
        /// 使用指定核尺寸对图像执行高斯滤波。
        /// </summary>
        /// <param name="image">待滤波的输入图像。</param>
        /// <param name="size">高斯滤波核尺寸。</param>
        /// <returns>高斯滤波后的新图像，输入为空时返回 null。</returns>
        public static HImage GaussImage(HImage image, int size)
        {
            if (image == null) return null;
            HObject result;
            HOperatorSet.GaussImage(image, out result, size);
            return new HImage(result);
        }
        #endregion
    }
}
