using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Events;

public delegate void InteractionChangedEventHandler(object sender, InteractionChangedEventArgs e);

public class InteractionChangedEventArgs
{
    public InteractionChangedEventArgs()
    {
    }


    public Vector2 GeoCoordinate
    {
        get; set;
    }

    public Vector2 WorldCoordinate
    {
        get; set;
    }

    public Vector2 ScreenCoordinate
    {
        get; set;
    }
}