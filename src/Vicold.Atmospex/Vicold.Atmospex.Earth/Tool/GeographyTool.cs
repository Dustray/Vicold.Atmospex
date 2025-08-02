using Vicold.Atmospex.Earth.Projection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Tool
{
    public static class GeographyTool
    {

        /// <summary>
        /// 跨经度边界分割线
        /// TODO: standard2.1中使用Span
        /// </summary>
        /// <param name="line">原始线，要求此线已经转换到正常范围</param>
        /// <returns></returns>
        public static List<float[]> CrossLonBorderSplitLine(float[] line, float cutIn)
        {
            return Algorithm.GeographyAlgorithm.CrossLonBorderSplitLine(line, cutIn);
        }

        /// <summary>
        /// 跨经度边界分割面
        /// </summary>
        /// <param name="polygen">原始面，要求此面上的点已经转换到正常范围</param>
        /// <returns></returns>
        public static List<float[]> CrossLonBorderSplitPolygon(float[] polygen, float cutIn)
        {
            return Algorithm.GeographyAlgorithm.CrossLonBorderSplitPolygon(polygen, cutIn);

        }

        public static string GeographyCoordinateToString(double lon, double lat, int keepPoint = 2)
        {
            string lonSymbol = lon == 0 ? "" : (lon < 0 ? "W" : "E");
            string latSymbol = lat == 0 ? "" : (lat < 0 ? "S" : "N");

            if (lon < -180 || lon > 180 || lat < -90 || lat > 90) return "";
            return $"{Math.Round(Math.Abs(lon), keepPoint, MidpointRounding.AwayFromZero)}°{lonSymbol}, {Math.Round(Math.Abs(lat), keepPoint, MidpointRounding.AwayFromZero)}°{latSymbol}";
        }

    }
}
