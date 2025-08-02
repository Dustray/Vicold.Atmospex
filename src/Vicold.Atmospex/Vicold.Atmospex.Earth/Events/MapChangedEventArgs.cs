using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Events;

public delegate void MapChangedEventHandler(object sender, MapChangedEventArgs e);

public class MapChangedEventArgs
{
    public MapChangedEventArgs(float scale)
    {
        Scale = scale;
    }


    public float Scale
    {
        get; set;
    }
}