using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Algorithm
{
    public static class GeographyAlgorithm
    {
        enum LineSide
        {
            None,
            Left,
            Right,
        }

        /// <summary>
        /// 将任意经度转换为要求范围内的经纬度
        /// </summary>
        /// <param name="value">原始经度</param>
        /// <param name="minLon">范围最小经度</param>
        /// <param name="maxLon">范围最小纬度</param>
        /// <returns>转换后的经度</returns>
        public static double StandardLongitudeConvert(double value, double minLon, double maxLon)
        {
            var standard = value % 360;
            if (standard > maxLon)
            {
                return standard - 360;
            }
            else if (standard < minLon)
            {
                return standard + 360;
            }
            else
            {
                return standard;
            }
        }

        /// <summary>
        /// 跨经度边界分割线
        /// TODO: standard2.1中使用Span
        /// </summary>
        /// <param name="line">原始线，要求此线已经转换到正常范围</param>
        /// <returns></returns>
        public static List<float[]> CrossLonBorderSplitLine(float[] line, float cutIn)
        {
            List<float[]> result = new List<float[]>();

            // 从第二个点开始遍历
            int lastLineStartPosition = 0;
            for (var i = 2; i < line.Length; i += 2)
            {
                var thisLon = line[i];
                var lastLon = line[i - 2];
                var isCrossed = IsCrossed2(lastLon, thisLon, cutIn);
                 
                if (isCrossed ) //两个本来相连的点突然离得特别远（>180），则说明这两个点被分割了, 这个180有争议的，可以再小点，但是不确定
                {
                    // 代表跨边界了，新建一条线
                    var len = i - lastLineStartPosition;
                    var newLine = new float[len];

                    Array.Copy(line, lastLineStartPosition, newLine, 0, len); // 用Span也是极好的，可惜用不了，唉，只能复制内存了
                    result.Add(newLine);

                    lastLineStartPosition = i;
                }
            }

            if (result.Count == 0)
            {
                result.Add(line);
            }
            else
            {
                var len = line.Length - lastLineStartPosition;
                var newLine = new float[len];

                Array.Copy(line, lastLineStartPosition, newLine, 0, len);
                result.Add(newLine);
            }

            return result;
        }

        /// <summary>
        /// 跨经度边界分割面
        /// </summary>
        /// <param name="polygon">原始面，要求此面上的点已经转换到正常范围</param>
        /// <returns></returns>
        public static List<float[]> CrossLonBorderSplitPolygon(float[] polygon, float cutIn)
        {
            List<float[]> result = new List<float[]>();
            int mainPolygonParseIndex = -1; // 主面暂停的地方
            int mainPolygonResumeIndex = -1; // 主面继续的地方
            for (var i = 0; i < polygon.Length - 2; i += 2)
            {
                var thisLon = polygon[i];
                var nextLon = polygon[i + 2];
                var isCrossed = IsCrossed(nextLon, thisLon, cutIn);
                if (isCrossed.Item2 == LineSide.None)
                {
                    continue;
                }
                if (isCrossed.Item1) // 两个本来相连的点突然离得特别远（>180），则说明这两个点被分割了
                {
                    i += 2;
                    mainPolygonParseIndex = i; // 主面暂停的地方

                    // 循环接下来的面
                    while (i != polygon.Length - 2)
                    {
                        // 代表下一个点跨边界了，新建一个面数组
                        var startIndex = i;
                        Dictionary<int, int> dd = new Dictionary<int, int>();
                        if(dd?.TryGetValue(1, out var value) ?? true)
                        {

                        }
                        // 开始循环新面的点
                        for (; i < polygon.Length - 2; i += 2)
                        {
                            var thisLon1 = polygon[i];
                            var nextLon1 = polygon[i + 2];
                            var isCrossed1 = IsCrossed(nextLon1, thisLon1, cutIn);
                            if (isCrossed1.Item2 == LineSide.None)
                            {
                                continue;
                            }
                            if (isCrossed1.Item1) // 又跨界了
                            {
                                i += 2;
                                break;
                            }
                        }

                        if (i == polygon.Length - 2)
                        {
                            break;
                        }

                        var endIndex = i;

                        var len = endIndex - startIndex;
                        var newLine = new float[len];
                        Array.Copy(polygon, startIndex, newLine, 0, len); // 用Span也是极好的，可惜用不了，唉，只能复制内存了
                        result.Add(newLine);

                        mainPolygonResumeIndex = endIndex; // 主面可能继续的地方
                    }
                }
            }

            if (mainPolygonParseIndex == -1)
            {
                // 没暂停过，无暂存落区
                result.Add(polygon);
            }
            else
            {
                // 把主面加进去
                var headLen = mainPolygonParseIndex;
                var tailLen = polygon.Length - mainPolygonResumeIndex;
                var newLine = new float[headLen + tailLen];

                Array.Copy(polygon, 0, newLine, 0, headLen); // 复制前半块
                Array.Copy(polygon, mainPolygonResumeIndex, newLine, headLen, tailLen); // 复制后半块
                result.Add(newLine);
            }

            return result;
        }


        private static  bool  IsCrossed2(float lon0, float lon1, float lonBorder)
        {
            //return Math.Abs(lon1 - lon2) > 180;
            //return (lon1 < cutIn & lon2 > cutIn) | (lon1 > cutIn & lon2 < cutIn);

            var side0 = GetSideOf(lon0, lonBorder);
            var side1 = GetSideOf(lon1, lonBorder);
            double delta = Math.Abs(lon0 - lon1);
            return  side0 != side1 && delta < 180  ;
        }

        private static (bool, LineSide) IsCrossed(float lon0, float lon1, float lonBorder)
        {
            //return Math.Abs(lon1 - lon2) > 180;
            //return (lon1 < cutIn & lon2 > cutIn) | (lon1 > cutIn & lon2 < cutIn);

            var side0 = GetSideOf(lon0, lonBorder);
            var side1 = GetSideOf(lon1, lonBorder);
            double delta = Math.Abs(lon0 - lon1);
            return ((side0 != side1 && delta < 180), side1);
        }
        private static LineSide GetSideOf(float x, float x0, double epsilon = 1e-5)
        {
            double c = (x - x0);
            if (c < -epsilon)
            {
                return LineSide.Left;
            }
            else if (c > epsilon)
            {
                return LineSide.Right;
            }
            return LineSide.None;

        }

    }
}
