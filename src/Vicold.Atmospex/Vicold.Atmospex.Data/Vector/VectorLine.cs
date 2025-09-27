using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Vicold.Atmospex.Data.LineData;

namespace Vicold.Atmospex.Data.Vector;
public class VectorLine
{

    private readonly Vector2[] _line;

    public VectorLine(int length)
    {
        _line = new Vector2[length];
    }

    public VectorLine(Vector2[] vector2s, Color lineColor, int lineWidth, Color fillColor)
    {
        _line = vector2s;
        LineColor = lineColor;
        FillColor = fillColor;
        LineWidth = lineWidth;
    }

    public PolygonType PolygonType
    {
        get;
        set;
    }

    public Color LineColor
    {
        get;
        set;
    }

    public Color FillColor
    {
        get;
        set;
    }

    public float Width
    {
        get;
        set;
    }

    public Vector2[] Data => _line;

    public int LineWidth
    {
        get;
        set;
    } = 1;

    // 新增最大最小值属性
    public float MinX { get; set; } = float.MaxValue;
    public float MinY { get; set; } = float.MaxValue;
    public float MaxX { get; set; } = float.MinValue;
    public float MaxY { get; set; } = float.MinValue;

    //public Vector2 this[int index]
    //{
    //    get
    //    {
    //        return Line[index];
    //    }
    //    set
    //    {
    //        Line[index] = value;
    //    }
    //}

    public void Set(int index, float x, float y)
    {
        _line[index] = new Vector2(x, y);
    }
}