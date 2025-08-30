using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.ImageMaker;
public struct ImageBound
{
    public double West;
    public double North;
    public double East;
    public double South;
    public double LonInterval;
    public double LatInterval;

    /// <summary>
    /// world bounds
    /// </summary>
    public double MinX, MinY;
    public double MaxX, MaxY;

    public int XOffset;
    public int YOffset;
    public int XSize;
    public int YSize;

    public bool IsValid
    {
        get
        {
            return !(MinX >= MaxX || MinY >= MaxY);
        }
    }
}