using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Algorithm;
public static class GraphAlgorithm
{

    /// <summary>
    /// 判断点是否在多边形内部（射线法）。
    /// </summary>
    public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            var pi = polygon[i];
            var pj = polygon[j];

            bool intersect =
                ((pi.Y > point.Y) != (pj.Y > point.Y)) &&
                (point.X < (pj.X - pi.X) * (point.Y - pi.Y) / (pj.Y - pi.Y + float.Epsilon) + pi.X);

            if (intersect)
                inside = !inside;
        }
        return inside;
    }
}
