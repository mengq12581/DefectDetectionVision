using System;
using HalconDotNet;

namespace VisionDemo2026
{
    public class HalconRegionHelper
    {
        #region 阈值分割
        /// <summary>
        /// 按灰度范围对图像执行固定阈值分割。
        /// </summary>
        /// <param name="image">待分割的灰度图像。</param>
        /// <param name="minGray">保留区域的最小灰度值。</param>
        /// <param name="maxGray">保留区域的最大灰度值。</param>
        /// <returns>分割得到的区域，输入为空时返回 null。</returns>
        public static HObject Threshold(HImage image, int minGray, int maxGray)
        {
            if (image == null) return null;
            HObject region;
            HOperatorSet.Threshold(image, out region, minGray, maxGray);
            return region;
        }
        #endregion

        #region 自动阈值
        /// <summary>
        /// 使用最大类间方差法自动分割亮目标。
        /// </summary>
        /// <param name="image">待分割的灰度图像。</param>
        /// <param name="usedThreshold">输出 HALCON 自动计算的阈值。</param>
        /// <returns>自动阈值分割得到的区域。</returns>
        public static HObject BinaryThreshold(HImage image, out int usedThreshold)
        {
            usedThreshold = 0;
            if (image == null) return null;
            HObject region;
            HTuple used;
            HOperatorSet.BinaryThreshold(image, out region, "max_separability", "light", out used);
            usedThreshold = used.I;
            return region;
        }
        #endregion

        #region 动态阈值
        /// <summary>
        /// 将输入图与背景图比较，执行局部动态阈值分割。
        /// </summary>
        /// <param name="image">待分割的灰度图像。</param>
        /// <param name="backgroundImage">用于局部比较的背景图像。</param>
        /// <param name="offset">前景与背景之间的灰度偏移阈值。</param>
        /// <param name="polarity">目标极性，例如 dark 或 light。</param>
        /// <returns>动态阈值分割得到的区域。</returns>
        public static HObject DynThreshold(HImage image, HImage backgroundImage, int offset, string polarity)
        {
            if (image == null || backgroundImage == null) return null;
            HObject region;
            HOperatorSet.DynThreshold(image, backgroundImage, out region, offset, polarity);
            return region;
        }
        #endregion

        #region 开运算
        /// <summary>
        /// 使用圆形结构元素对区域执行开运算。
        /// </summary>
        /// <param name="region">待处理的区域。</param>
        /// <param name="radius">圆形结构元素半径。</param>
        /// <returns>开运算后的新区域。</returns>
        public static HObject OpeningCircle(HObject region, double radius)
        {
            if (region == null) return null;
            HObject result;
            HOperatorSet.OpeningCircle(region, out result, radius);
            return result;
        }
        #endregion

        #region 闭运算
        /// <summary>
        /// 使用圆形结构元素对区域执行闭运算。
        /// </summary>
        /// <param name="region">待处理的区域。</param>
        /// <param name="radius">圆形结构元素半径。</param>
        /// <returns>闭运算后的新区域。</returns>
        public static HObject ClosingCircle(HObject region, double radius)
        {
            if (region == null) return null;
            HObject result;
            HOperatorSet.ClosingCircle(region, out result, radius);
            return result;
        }
        #endregion

        #region 填充
        /// <summary>
        /// 填充区域内部的孔洞。
        /// </summary>
        /// <param name="region">待填充的区域。</param>
        /// <returns>填充后的新区域。</returns>
        public static HObject FillUp(HObject region)
        {
            if (region == null) return null;
            HObject result;
            HOperatorSet.FillUp(region, out result);
            return result;
        }
        #endregion

        #region 连通域
        /// <summary>
        /// 将输入区域拆分为独立连通区域。
        /// </summary>
        /// <param name="region">待分析的区域。</param>
        /// <returns>连通区域集合。</returns>
        public static HObject Connection(HObject region)
        {
            if (region == null) return null;
            HObject result;
            HOperatorSet.Connection(region, out result);
            return result;
        }
        #endregion

        #region 筛选形状
        /// <summary>
        /// 按指定形状特征和数值范围筛选区域。
        /// </summary>
        /// <param name="regions">待筛选的区域集合。</param>
        /// <param name="criteria">HALCON 形状特征名称，例如 area。</param>
        /// <param name="operation">多个特征条件之间的逻辑操作。</param>
        /// <param name="minValue">特征允许的最小值。</param>
        /// <param name="maxValue">特征允许的最大值。</param>
        /// <returns>满足条件的区域集合。</returns>
        public static HObject SelectShape(HObject regions, string criteria, string operation, double minValue, double maxValue)
        {
            if (regions == null) return null;
            HObject result;
            HOperatorSet.SelectShape(regions, out result, criteria, operation, minValue, maxValue);
            return result;
        }
        #endregion

        #region 选择最大面积
        /// <summary>
        /// 从区域集合中选择面积最大的区域。
        /// </summary>
        /// <param name="regions">待筛选的区域集合。</param>
        /// <returns>面积最大的区域。</returns>
        public static HObject SelectMaxArea(HObject regions)
        {
            if (regions == null) return null;
            HObject result;
            HOperatorSet.SelectShapeStd(regions, out result, "max_area", 70);
            return result;
        }
        #endregion

        #region 排序区域
        /// <summary>
        /// 按指定模式和方向对区域集合排序。
        /// </summary>
        /// <param name="regions">待排序的区域集合。</param>
        /// <param name="sortMode">HALCON 区域排序模式。</param>
        /// <param name="ascending">是否按升序排列。</param>
        /// <returns>排序后的区域集合。</returns>
        public static HObject SortRegion(HObject regions, string sortMode, bool ascending)
        {
            if (regions == null) return null;
            HObject result;
            HOperatorSet.SortRegion(regions, out result, sortMode, ascending ? "true" : "false", "row");
            return result;
        }
        #endregion

        #region 面积中心
        /// <summary>
        /// 计算区域面积和重心坐标。
        /// </summary>
        /// <param name="region">待计算的区域。</param>
        /// <param name="area">输出区域面积。</param>
        /// <param name="row">输出重心行坐标。</param>
        /// <param name="col">输出重心列坐标。</param>
        public static void AreaCenter(HObject region, out double area, out double row, out double col)
        {
            area = 0; row = 0; col = 0;
            if (region == null) return;
            HTuple a, r, c;
            HOperatorSet.AreaCenter(region, out a, out r, out c);
            area = a.D;
            row = r.D;
            col = c.D;
        }
        #endregion

        #region 最小外接矩形
        /// <summary>
        /// 计算区域的轴对齐最小外接矩形。
        /// </summary>
        /// <param name="region">待计算的区域。</param>
        /// <param name="row1">输出左上角行坐标。</param>
        /// <param name="col1">输出左上角列坐标。</param>
        /// <param name="row2">输出右下角行坐标。</param>
        /// <param name="col2">输出右下角列坐标。</param>
        public static void SmallestRectangle1(HObject region, out double row1, out double col1, out double row2, out double col2)
        {
            row1 = col1 = row2 = col2 = 0;
            if (region == null) return;
            HTuple r1, c1, r2, c2;
            HOperatorSet.SmallestRectangle1(region, out r1, out c1, out r2, out c2);
            row1 = r1.D; col1 = c1.D; row2 = r2.D; col2 = c2.D;
        }
        #endregion

        #region 最小外接圆
        /// <summary>
        /// 计算区域的最小外接圆。
        /// </summary>
        /// <param name="region">待计算的区域。</param>
        /// <param name="row">输出圆心行坐标。</param>
        /// <param name="col">输出圆心列坐标。</param>
        /// <param name="radius">输出外接圆半径。</param>
        public static void SmallestCircle(HObject region, out double row, out double col, out double radius)
        {
            row = col = radius = 0;
            if (region == null) return;
            HTuple r, c, rad;
            HOperatorSet.SmallestCircle(region, out r, out c, out rad);
            row = r.D; col = c.D; radius = rad.D;
        }
        #endregion
    }
}
