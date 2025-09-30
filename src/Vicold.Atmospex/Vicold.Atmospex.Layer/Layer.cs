using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Providers;

namespace Vicold.Atmospex.Layer;

public abstract class Layer : ILayer
{
    protected internal ILayerNode? _layerNode;

    private string? _name;

    public string Name
    {
        set => _name = value;
        get => _name ?? DataProvider?.Name ?? "Undefined";
    }

    public string ID { get; set; } = Guid.NewGuid().ToString();

    public IStyle? Style
    {
        get; set;
    }

    public IProvider? DataProvider
    {
        get; set;
    }

    public int ZIndex
    {
        get; set;
    }

    public LayerLevel LayerZLevel
    {
        get; set;
    }

    public bool IsVisible
    {
        get => _layerNode?.Visible ?? false;
        set
        {
            if (_layerNode is { } && _layerNode.Visible != value)
            {
                _layerNode.Visible = value;
            }
        }
    }

    public bool IsSystemLayer => ID.StartsWith("rmias");

    public virtual void Render(IProjection projection)
    {
        if (DataProvider == null)
        {
            throw new Exception("未指定Provider");
        }

        if (!DataProvider.IsLoaded)
        {
            DataProvider.LoadData();
        }
    }
    public void Dispose()
    {
        _layerNode?.Dispose();
        DataProvider?.Dispose();
    }

    public abstract void ScaleChange(float scale);
}
