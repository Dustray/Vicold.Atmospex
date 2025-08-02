using Vicold.Atmospex.Earth.Projection;
using System;
using System.Collections.Generic;
using System.Text;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Render.Frame;

namespace Vicold.Atmospex.Layer
{
    public sealed class FontLayer : Layer
    {
        public FontLayer(IVectorDataProvider provider,string id)
        {
            DataProvider = provider;
            LayerZLevel = LayerLevel.Highest;
            ID = id;
        }

        public override void Render(IProjection projection)
        {
            base.Render(projection);
            var provider = (IVectorDataProvider)DataProvider;
            var texture = NodeFactory.CreateVectorsNode(ID, provider, projection);
            texture.SetLevel((int)LayerZLevel + ZIndex);
            _layerNode = texture;
        }

    }
}
