using Evergine.Common.Graphics;
using Evergine.Mathematics;

namespace Vicold.Atmospex.Render.ElementEntites;

public class ElementLine
{
    public Vector3 Begin
    {
        get; set;
    }

    public Vector3 End
    {
        get; set;
    }

    public Color Color
    {
        get; set;
    }

    public bool IsVisibile
    {
        get; set;
    }
}

