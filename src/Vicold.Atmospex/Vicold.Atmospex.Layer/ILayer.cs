using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Style;
using System;
using System.Collections.Generic;
using System.Text;
using Vicold.Atmospex.Data.Providers;

namespace Vicold.Atmospex.Layer;

public interface ILayer : IDisposable
{
    string Name
    {
        get; set;
    }

    string ID
    {
        get; set;
    }

    int ZIndex
    {
        get; set;
    }

    bool IsVisible
    {
        get;
    }

    bool IsSystemLayer
    {
        get;
    }

    IStyle? Style
    {
        get; set;
    }

    LayerLevel LayerZLevel
    {
        get; set;
    }

    IProvider? DataProvider
    {
        get; set;
    }

    void Render(IProjection projection);
}
