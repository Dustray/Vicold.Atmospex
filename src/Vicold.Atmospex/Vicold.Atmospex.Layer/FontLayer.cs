using System;
using System.Collections.Generic;
using System.Text;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Style;

namespace Vicold.Atmospex.Layer;

public class FontLayer : Layer
{
    public FontLayer(IVectorDataProvider provider, string id)
    {
        DataProvider = provider;
        LayerZLevel = LayerLevel.Highest;
        ID = id;
    }

    protected virtual ILayerNode? CreateVectorsNode(string ID, IVectorDataProvider provider, IProjection proj)
    {
        return null;
    }

    public override void Render(IProjection projection)
    {
        base.Render(projection);
        if (DataProvider is IVectorDataProvider provider)
        {
            var texture = CreateVectorsNode(ID, provider, projection);
            if (texture is { })
            {
                texture.SetLevel((int)LayerZLevel + ZIndex);
                _layerNode = texture;
            }
        }
    }

}
