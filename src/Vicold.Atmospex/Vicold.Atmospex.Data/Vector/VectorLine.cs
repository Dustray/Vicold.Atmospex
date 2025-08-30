using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Data.Vector;
public class VectorLine
{

    private readonly Vector2[] _line;

    public VectorLine(int length)
    {
        _line = new Vector2[length];
    }

    public PolygonType PolygonType
    {
        get; set;
    }

    public Color LineColor
    {
        get; set;
    }

    public Color FillColor
    {
        get; set;
    }

    public float Width
    {
        get; set;
    }

    public Vector2[] Data => _line;
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