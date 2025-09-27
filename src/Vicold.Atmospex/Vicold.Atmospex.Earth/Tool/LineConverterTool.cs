using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Earth.Projection;

namespace Vicold.Atmospex.Earth.Tool;
public static class LineConverterTool
{
    //private List<VectorLine> LoadToMap(Dictionary<int, List<float[]>> lineDic, IProjection proj)
    //{
    //    var lines = ToPointList(lineDic, proj);
    //    return lines;
    //}

    //private List<VectorLine> ToPointList(Dictionary<int, List<float[]>> dic, IProjection proj)
    //{
    //    var list = new List<VectorLine>();
    //    foreach (var lines in dic)
    //    {
    //        foreach (var line in lines.Value)
    //        {
    //            var length = line.Length / 2;
    //            var points = new VectorLine(length);
    //            var newIndex = 0;
    //            for (var i = 0; i < length; i++)
    //            {
    //                proj.Geo2Index(line[2 * i], line[2 * i + 1], out var x, out var y);
    //                points.Set(newIndex, (float)x, (float)y);
    //                newIndex++;
    //            }
    //            list.Add(points);
    //        }
    //    }
    //    return list;
    //}


    public static VectorLine[] ToVectorLines(LineData dic, IProjection proj)
    {
        var list = new VectorLine[dic.Count];
        for (var i = 0; i < dic.Count; i++)
        {
            var line = dic[i];
            var length = line.DataSpan.Length / 2;
            var span = line.DataSpan.Span;
            var points = new VectorLine(length);
            points.LineColor = line.Color;
            points.FillColor = line.FillColor;
            points.PolygonType = line.PolygonType;
            points.Width = line.Width;
            
            // 初始化最大最小值
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            
            var newIndex = 0;
            for (var j = 0; j < length; j++)
            {
                // 转换坐标
                proj.Geo2Index(span[2 * j], span[2 * j + 1], out var x, out var y);
                var projX = (float)x;
                var projY = (float)y;
                
                // 更新最大最小值
                minX = Math.Min(minX, projX);
                minY = Math.Min(minY, projY);
                maxX = Math.Max(maxX, projX);
                maxY = Math.Max(maxY, projY);
                
                // 设置点坐标
                points.Set(newIndex, projX, projY);
                newIndex++;
            }
            
            // 赋值最大最小值
            points.MinX = minX;
            points.MinY = minY;
            points.MaxX = maxX;
            points.MaxY = maxY;
            
            list[i] = points;
        }
        return list;
    }
}
