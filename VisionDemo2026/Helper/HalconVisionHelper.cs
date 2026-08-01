using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HalconDotNet;
using Sunny.UI;

namespace VisionDemo2026
{
    public class HalconVisionHelper
    {
        #region 获取模板路径
        /// <summary>
        /// 获取形状模板文件的完整路径，并在需要时创建目录。
        /// </summary>
        /// <param name="folder">相对于应用程序目录的模板文件夹。</param>
        /// <param name="fileName">形状模板文件名。</param>
        /// <returns>形状模板文件的完整路径。</returns>
        public static string GetShapeModelPath(string folder, string fileName)
        {
            string dir = Path.Combine(Application.StartupPath, folder);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return Path.Combine(dir, fileName);
        }

        /// <summary>
        /// 获取指定目录中所有形状模板名称并按名称排序。
        /// </summary>
        /// <param name="folder">相对于应用程序目录的模板文件夹。</param>
        /// <returns>不含扩展名的形状模板名称列表。</returns>
        public static List<string> GetShapeModelNames(string folder)
        {
            string dir = Path.Combine(Application.StartupPath, folder);
            if (!Directory.Exists(dir))
                return new List<string>();

            return Directory.GetFiles(dir, "*.shm", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// 获取 OCR 模型文件的完整路径，并在需要时创建目录。
        /// </summary>
        /// <param name="folder">相对于应用程序目录的 OCR 模型文件夹。</param>
        /// <param name="fileName">OCR 模型文件名。</param>
        /// <returns>OCR 模型文件的完整路径。</returns>
        public static string GetOcrModelPath(string folder, string fileName)
        {
            string dir = Path.Combine(Application.StartupPath, folder);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return Path.Combine(dir, fileName);
        }
        #endregion

        #region 读取 OCR 模型
        /// <summary>
        /// 从文件读取 HALCON MLP OCR 分类器。
        /// </summary>
        /// <param name="fileName">OCR 模型文件完整路径。</param>
        /// <param name="handle">输出 OCR 分类器句柄。</param>
        public static void OcrRead(string fileName, out HTuple handle)
        {
            HOperatorSet.ReadOcrClassMlp(fileName, out handle);
        }
        #endregion

        #region OCR 识别
        /// <summary>
        /// 使用 MLP OCR 模型识别多个字符区域。
        /// </summary>
        /// <param name="regions">按字符分割的区域集合。</param>
        /// <param name="image">包含字符的输入图像。</param>
        /// <param name="handle">已经读取的 OCR 分类器句柄。</param>
        /// <param name="classes">输出识别到的字符类别。</param>
        /// <param name="confidences">输出各字符的识别置信度。</param>
        public static void OcrRecognize(HObject regions, HImage image, HTuple handle,
                                         out HTuple classes, out HTuple confidences)
        {
            HOperatorSet.DoOcrMultiClassMlp(regions, image, handle, out classes, out confidences);
        }
        #endregion

        #region 清除 OCR 模型
        /// <summary>
        /// 释放 HALCON MLP OCR 分类器句柄。
        /// </summary>
        /// <param name="handle">需要释放的 OCR 分类器句柄。</param>
        public static void OcrClear(HTuple handle)
        {
            HOperatorSet.ClearOcrClassMlp(handle);
        }
        #endregion

        #region 创建模板
        /// <summary>
        /// 在指定模板区域内创建可旋转的 HALCON 形状模板。
        /// </summary>
        /// <param name="image">用于创建模板的源图像。</param>
        /// <param name="templateRegion">限定模板内容的区域。</param>
        /// <param name="modelId">输出创建后的形状模板句柄。</param>
        public static void ShapeCreate(HImage image, HObject templateRegion,
                                        out HTuple modelId)
        {
            // 先把图像裁剪到模板区域
            HObject croppedImage;
            HOperatorSet.ReduceDomain(image, templateRegion, out croppedImage);

            // 用裁剪后的小图创建模板
            HOperatorSet.CreateShapeModel(
                croppedImage,    // 裁剪后的图像
                "auto",          // 金字塔层数（自动）
                0,    // 起始角度（-45度）
                360,     // 角度范围（90度，-45~+45）
                "auto",          // 角度步长（自动）
                "auto",          // 优化方式
                "use_polarity",  // 极性匹配
                "auto",          // 对比度
                "auto",          // 最小对比度
                out modelId);

            // 释放裁剪图
            croppedImage.Dispose();
        }
        #endregion

        #region 获取模板轮廓
        /// <summary>
        /// 获取形状模板第一层金字塔的 XLD 轮廓。
        /// </summary>
        /// <param name="modelId">形状模板句柄。</param>
        /// <param name="contours">输出模板轮廓对象。</param>
        public static void ShapeGetContours(HTuple modelId, out HObject contours)
        {
            HOperatorSet.GetShapeModelContours(out contours, modelId, new HTuple(1));
        }
        #endregion

        #region 读取模板
        /// <summary>
        /// 从文件读取 HALCON 形状模板。
        /// </summary>
        /// <param name="fileName">形状模板文件完整路径。</param>
        /// <param name="modelId">输出形状模板句柄。</param>
        public static void ShapeRead(string fileName, out HTuple modelId)
        {
            HOperatorSet.ReadShapeModel(fileName, out modelId);
        }
        #endregion

        #region 查找模板
        /// <summary>
        /// 在图像中查找指定数量和最低分数的形状模板实例。
        /// </summary>
        /// <param name="modelId">形状模板句柄。</param>
        /// <param name="image">待匹配的输入图像。</param>
        /// <param name="minScore">匹配结果允许的最低分数。</param>
        /// <param name="numMatches">需要返回的最大匹配数量，0 表示全部。</param>
        /// <param name="rows">输出匹配中心的行坐标。</param>
        /// <param name="cols">输出匹配中心的列坐标。</param>
        /// <param name="angles">输出各匹配实例的旋转角度。</param>
        /// <param name="scores">输出各匹配实例的分数。</param>
        public static void ShapeFind(HTuple modelId, HImage image, double minScore, int numMatches,
                                      out HTuple rows, out HTuple cols, out HTuple angles, out HTuple scores)
        {
            HOperatorSet.FindShapeModel(image, modelId,
                                        new HTuple(0), new HTuple(360),
                                        new HTuple(minScore), new HTuple(numMatches), new HTuple(0.5),
                                        new HTuple("least_squares"), new HTuple(0), new HTuple(0.5),
                                        out rows, out cols, out angles, out scores);
        }
        #endregion

        #region 保存模板
        /// <summary>
        /// 将 HALCON 形状模板写入文件。
        /// </summary>
        /// <param name="modelId">需要保存的形状模板句柄。</param>
        /// <param name="fileName">模板输出文件完整路径。</param>
        public static void ShapeWrite(HTuple modelId, string fileName)
        {
            HOperatorSet.WriteShapeModel(modelId, fileName);
        }
        #endregion

        #region 清除模板
        /// <summary>
        /// 释放 HALCON 形状模板句柄。
        /// </summary>
        /// <param name="modelId">需要释放的形状模板句柄。</param>
        public static void ShapeClear(HTuple modelId)
        {
            HOperatorSet.ClearShapeModel(modelId);
        }
        #endregion

        #region 创建测量模型
        /// <summary>
        /// 创建一个空的 HALCON 计量模型。
        /// </summary>
        /// <param name="handle">输出计量模型句柄。</param>
        public static void MetrologyCreate(out HTuple handle)
        {
            HOperatorSet.CreateMetrologyModel(out handle);
        }
        #endregion

        #region 添加直线测量
        /// <summary>
        /// 向计量模型添加一条直线测量对象。
        /// </summary>
        /// <param name="handle">计量模型句柄。</param>
        /// <param name="r1">初始直线起点的行坐标。</param>
        /// <param name="c1">初始直线起点的列坐标。</param>
        /// <param name="r2">初始直线终点的行坐标。</param>
        /// <param name="c2">初始直线终点的列坐标。</param>
        /// <param name="index">输出新增测量对象的索引。</param>
        public static void MetrologyAddLine(HTuple handle, double r1, double c1, double r2, double c2, out int index)
        {
            HTuple idx;
            HOperatorSet.AddMetrologyObjectLineMeasure(handle, r1, c1, r2, c2,
                new HTuple(20), new HTuple(5), new HTuple(1), new HTuple(30),
                new HTuple(), new HTuple(), out idx);
            index = idx.I;
        }
        #endregion

        #region 添加圆测量
        /// <summary>
        /// 向计量模型添加一个圆测量对象。
        /// </summary>
        /// <param name="handle">计量模型句柄。</param>
        /// <param name="row">初始圆心行坐标。</param>
        /// <param name="col">初始圆心列坐标。</param>
        /// <param name="radius">初始圆半径。</param>
        /// <param name="index">输出新增测量对象的索引。</param>
        public static void MetrologyAddCircle(HTuple handle, double row, double col, double radius, out int index)
        {
            HTuple idx;
            HOperatorSet.AddMetrologyObjectCircleMeasure(handle, row, col, radius,
                new HTuple(20), new HTuple(5), new HTuple(1), new HTuple(30),
                new HTuple(), new HTuple(), out idx);
            index = idx.I;
        }
        #endregion

        #region 应用测量
        /// <summary>
        /// 将计量模型应用到输入图像。
        /// </summary>
        /// <param name="image">需要测量的输入图像。</param>
        /// <param name="handle">已经配置测量对象的计量模型句柄。</param>
        public static void MetrologyApply(HImage image, HTuple handle)
        {
            HOperatorSet.ApplyMetrologyModel(image, handle);
        }
        #endregion

        #region 获取圆测量结果
        /// <summary>
        /// 获取指定圆测量对象的圆心和半径。
        /// </summary>
        /// <param name="handle">已经执行测量的计量模型句柄。</param>
        /// <param name="index">圆测量对象索引。</param>
        /// <param name="row">输出圆心行坐标。</param>
        /// <param name="col">输出圆心列坐标。</param>
        /// <param name="radius">输出测量得到的圆半径。</param>
        public static void MetrologyGetCircle(HTuple handle, int index,
                                               out double row, out double col, out double radius)
        {
            HTuple data;
            HOperatorSet.GetMetrologyObjectResult(handle, index, "all", "result_type", "all_param", out data);
            row = data[0].D;
            col = data[1].D;
            radius = data[2].D;
        }
        #endregion

        #region 获取直线测量结果
        /// <summary>
        /// 获取指定直线测量对象的起点和终点坐标。
        /// </summary>
        /// <param name="handle">已经执行测量的计量模型句柄。</param>
        /// <param name="index">直线测量对象索引。</param>
        /// <param name="r1">输出直线起点行坐标。</param>
        /// <param name="c1">输出直线起点列坐标。</param>
        /// <param name="r2">输出直线终点行坐标。</param>
        /// <param name="c2">输出直线终点列坐标。</param>
        public static void MetrologyGetLine(HTuple handle, int index,
                                             out double r1, out double c1, out double r2, out double c2)
        {
            HTuple data;
            HOperatorSet.GetMetrologyObjectResult(handle, index, "all", "result_type", "all_param", out data);
            r1 = data[0].D;
            c1 = data[1].D;
            r2 = data[2].D;
            c2 = data[3].D;
        }
        #endregion

        #region 清除测量模型
        /// <summary>
        /// 释放 HALCON 计量模型句柄。
        /// </summary>
        /// <param name="handle">需要释放的计量模型句柄。</param>
        public static void MetrologyClear(HTuple handle)
        {
            HOperatorSet.ClearMetrologyModel(handle);
        }
        #endregion

        #region 绘制矩形
        /// <summary>
        /// 在 HALCON 窗口中交互绘制轴对齐矩形。
        /// </summary>
        /// <param name="window">用于交互绘制的 HALCON 窗口控件。</param>
        /// <param name="row1">输出左上角行坐标。</param>
        /// <param name="col1">输出左上角列坐标。</param>
        /// <param name="row2">输出右下角行坐标。</param>
        /// <param name="col2">输出右下角列坐标。</param>
        /// <returns>窗口有效且绘制完成时返回 true。</returns>
        public static bool DrawRectangle1(HWindowControl window,
                                          out double row1, out double col1,
                                          out double row2, out double col2)
        {
            row1 = col1 = row2 = col2 = 0;
            if (window == null) return false;

            double r1 = 0, c1 = 0, r2 = 0, c2 = 0;
            window.HalconWindow.SetColor("red");
            window.HalconWindow.SetDraw("margin");
            UIMessageTip.Show("请绘制矩形");
            window.HalconWindow.DrawRectangle1(out r1, out c1, out r2, out c2);
            row1 = r1; col1 = c1; row2 = r2; col2 = c2;
            return true;
        }
        #endregion

        #region 绘制圆
        /// <summary>
        /// 在 HALCON 窗口中交互绘制圆。
        /// </summary>
        /// <param name="window">用于交互绘制的 HALCON 窗口控件。</param>
        /// <param name="row">输出圆心行坐标。</param>
        /// <param name="col">输出圆心列坐标。</param>
        /// <param name="radius">输出圆半径。</param>
        /// <returns>窗口有效且绘制完成时返回 true。</returns>
        public static bool DrawCircle(HWindowControl window,
                                       out double row, out double col, out double radius)
        {
            row = col = radius = 0;
            if (window == null) return false;

            double r = 0, c = 0, rad = 0;
            window.HalconWindow.SetColor("red");
            window.HalconWindow.SetDraw("margin");
            window.HalconWindow.DrawCircle(out r, out c, out rad);
            row = r; col = c; radius = rad;
            return true;
        }
        #endregion

        #region 绘制直线
        /// <summary>
        /// 在 HALCON 窗口中交互绘制直线。
        /// </summary>
        /// <param name="window">用于交互绘制的 HALCON 窗口控件。</param>
        /// <param name="row1">输出直线起点行坐标。</param>
        /// <param name="col1">输出直线起点列坐标。</param>
        /// <param name="row2">输出直线终点行坐标。</param>
        /// <param name="col2">输出直线终点列坐标。</param>
        /// <returns>窗口有效且绘制完成时返回 true。</returns>
        public static bool DrawLine(HWindowControl window,
                                     out double row1, out double col1,
                                     out double row2, out double col2)
        {
            row1 = col1 = row2 = col2 = 0;
            if (window == null) return false;

            double r1 = 0, c1 = 0, r2 = 0, c2 = 0;
            window.HalconWindow.SetColor("red");
            window.HalconWindow.SetDraw("margin");
            window.HalconWindow.DrawLine(out r1, out c1, out r2, out c2);
            row1 = r1; col1 = c1; row2 = r2; col2 = c2;
            return true;
        }
        #endregion

        #region 绘制点
        /// <summary>
        /// 在 HALCON 窗口中交互选择一个点。
        /// </summary>
        /// <param name="window">用于交互选点的 HALCON 窗口控件。</param>
        /// <param name="row">输出点的行坐标。</param>
        /// <param name="col">输出点的列坐标。</param>
        /// <returns>窗口有效且选点完成时返回 true。</returns>
        public static bool DrawPoint(HWindowControl window, out double row, out double col)
        {
            row = col = 0;
            if (window == null) return false;

            double r = 0, c = 0;
            window.HalconWindow.SetColor("red");
            window.HalconWindow.DrawPoint(out r, out c);
            row = r; col = c;
            return true;
        }
        #endregion

        #region 生成矩形
        /// <summary>
        /// 根据两个角点坐标生成轴对齐矩形区域。
        /// </summary>
        /// <param name="row1">左上角行坐标。</param>
        /// <param name="col1">左上角列坐标。</param>
        /// <param name="row2">右下角行坐标。</param>
        /// <param name="col2">右下角列坐标。</param>
        /// <returns>生成的矩形区域。</returns>
        public static HObject GenRectangle1(double row1, double col1, double row2, double col2)
        {
            HObject region;
            HOperatorSet.GenRectangle1(out region, row1, col1, row2, col2);
            return region;
        }
        #endregion

        #region 生成圆
        /// <summary>
        /// 根据圆心和半径生成圆形区域。
        /// </summary>
        /// <param name="row">圆心行坐标。</param>
        /// <param name="col">圆心列坐标。</param>
        /// <param name="radius">圆半径。</param>
        /// <returns>生成的圆形区域。</returns>
        public static HObject GenCircle(double row, double col, double radius)
        {
            HObject region;
            HOperatorSet.GenCircle(out region, row, col, radius);
            return region;
        }
        #endregion

        #region 显示 XLD 轮廓
        /// <summary>
        /// 使用指定颜色和线宽显示 XLD 轮廓。
        /// </summary>
        /// <param name="window">目标 HALCON 窗口控件。</param>
        /// <param name="contour">需要显示的 XLD 轮廓对象。</param>
        /// <param name="color">HALCON 支持的显示颜色名称。</param>
        /// <param name="lineWidth">轮廓显示线宽。</param>
        public static void DisplayContour(HWindowControl window, HObject contour, string color, double lineWidth)
        {
            if (window == null || contour == null) return;
            window.HalconWindow.SetColor(color);
            window.HalconWindow.SetLineWidth(lineWidth);
            window.HalconWindow.DispXld(new HXLD(contour));
        }
        #endregion
    }
}
