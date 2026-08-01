using System;
using System.Collections.Generic;
using HalconDotNet;

namespace VisionDemo2026
{
    public class HalconCalibrationHelper
    {
        private class PointD
        {
            public double X;
            public double Y;
            /// <summary>
            /// 创建一个二维标定点。
            /// </summary>
            /// <param name="x">点的横坐标。</param>
            /// <param name="y">点的纵坐标。</param>
            public PointD(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        private static readonly List<PointD> PixelPoints = new List<PointD>();
        private static readonly List<PointD> RobotPoints = new List<PointD>();
        private static HTuple HomMat2D = null;
        private static double CalibratedAngle = 0;

        #region 开始标定
        /// <summary>
        /// 清空历史标定点、仿射矩阵和标定角度，开始新一轮标定。
        /// </summary>
        public static void CalibrationBegin()
        {
            PixelPoints.Clear();
            RobotPoints.Clear();
            HomMat2D = null;
            CalibratedAngle = 0;
        }
        #endregion

        #region 加一个标定点
        /// <summary>
        /// 添加一组对应的像素坐标和机器人坐标。
        /// </summary>
        /// <param name="pixelX">图像点的横坐标。</param>
        /// <param name="pixelY">图像点的纵坐标。</param>
        /// <param name="robotX">对应机器人点的 X 坐标。</param>
        /// <param name="robotY">对应机器人点的 Y 坐标。</param>
        public static void CalibrationAddPoint(double pixelX, double pixelY,
                                              double robotX, double robotY)
        {
            PixelPoints.Add(new PointD(pixelX, pixelY));
            RobotPoints.Add(new PointD(robotX, robotY));
        }
        #endregion

        #region 计算仿射矩阵
        /// <summary>
        /// 根据已添加的标定点计算二维仿射矩阵和旋转角度。
        /// </summary>
        public static void CalibrationEnd()
        {
            if (PixelPoints.Count < 3)
                throw new Exception("标定点太少，至少 3 个");

            HTuple px = new HTuple();
            HTuple py = new HTuple();
            HTuple rx = new HTuple();
            HTuple ry = new HTuple();

            for (int i = 0; i < PixelPoints.Count; i++)
            {
                px = px.TupleConcat(PixelPoints[i].X);
                py = py.TupleConcat(PixelPoints[i].Y);
                rx = rx.TupleConcat(RobotPoints[i].X);
                ry = ry.TupleConcat(RobotPoints[i].Y);
            }

            HOperatorSet.VectorToHomMat2d(px, py, rx, ry, out HomMat2D);

            double a = HomMat2D[0].D;
            double b = HomMat2D[1].D;
            CalibratedAngle = Math.Atan2(b, a);
        }
        #endregion

        #region 像素坐标 → 机器人坐标
        /// <summary>
        /// 将像素坐标转换为机器人坐标，并返回标定旋转角度。
        /// </summary>
        /// <param name="pixelX">待转换点的像素横坐标。</param>
        /// <param name="pixelY">待转换点的像素纵坐标。</param>
        /// <param name="robotX">输出机器人 X 坐标。</param>
        /// <param name="robotY">输出机器人 Y 坐标。</param>
        /// <param name="robotAngle">输出标定得到的旋转角度，单位为弧度。</param>
        public static void PixelToRobot(double pixelX, double pixelY,
                                        out double robotX, out double robotY,
                                        out double robotAngle)
        {
            HOperatorSet.AffineTransPoint2d(HomMat2D,
                new HTuple(pixelX), new HTuple(pixelY),
                out HTuple outX, out HTuple outY);

            robotX = outX.D;
            robotY = outY.D;
            robotAngle = CalibratedAngle;
        }

        /// <summary>
        /// 将像素坐标转换为机器人二维坐标。
        /// </summary>
        /// <param name="pixelX">待转换点的像素横坐标。</param>
        /// <param name="pixelY">待转换点的像素纵坐标。</param>
        /// <param name="robotX">输出机器人 X 坐标。</param>
        /// <param name="robotY">输出机器人 Y 坐标。</param>
        public static void PixelToRobot(double pixelX, double pixelY,
                                        out double robotX, out double robotY)
        {
            HOperatorSet.AffineTransPoint2d(HomMat2D,
                new HTuple(pixelX), new HTuple(pixelY),
                out HTuple outX, out HTuple outY);

            robotX = outX.D;
            robotY = outY.D;
        }
        #endregion

        #region 获取标定误差
        /// <summary>
        /// 计算所有标定点转换结果的平均欧氏距离误差。
        /// </summary>
        /// <returns>机器人坐标系下的平均标定误差。</returns>
        public static double GetError()
        {
            if (HomMat2D == null)
                throw new Exception("请先调用 CalibrationEnd 完成标定");

            double total = 0;
            int count = PixelPoints.Count;

            for (int i = 0; i < count; i++)
            {
                double calcX, calcY;
                PixelToRobot(PixelPoints[i].X, PixelPoints[i].Y,
                    out calcX, out calcY);

                double dx = calcX - RobotPoints[i].X;
                double dy = calcY - RobotPoints[i].Y;
                total += Math.Sqrt(dx * dx + dy * dy);
            }

            return total / count;
        }
        #endregion

        #region 显示标定点
        /// <summary>
        /// 在 HALCON 窗口中以红色十字显示全部像素标定点。
        /// </summary>
        /// <param name="window">用于显示标定点的 HALCON 窗口控件。</param>
        public static void ShowPoints(HWindowControl window)
        {
            if (window == null) return;
            window.HalconWindow.SetColor("red");
            window.HalconWindow.SetDraw("margin");

            for (int i = 0; i < PixelPoints.Count; i++)
            {
                HObject cross;
                HOperatorSet.GenCrossContourXld(out cross,
                    PixelPoints[i].Y, PixelPoints[i].X, 20, 0);
                window.HalconWindow.DispObj(cross);
                cross.Dispose();
            }
        }
        #endregion
    }
}
