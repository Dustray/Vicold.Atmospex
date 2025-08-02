using Vicold.Atmospex.Earth.Projection;
using System;
using System.Collections.Generic;
using System.Text;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Render.Frame;

namespace Vicold.Atmospex.Layer
{
    public sealed class LineLayer : Layer
    {
        public LineLayer(IVectorDataProvider provider, string id)
        {
            DataProvider = provider;
            LayerZLevel = LayerLevel.High;
            ID = id;
        }

        public override void Render(IProjection projection)
        {
            base.Render(projection);

            var provider = (IVectorDataProvider)DataProvider;
            var texture = NodeFactory.CreateLinesNode(ID, provider, projection);
            if (texture != null)
            {
                texture.SetLevel((int)LayerZLevel + ZIndex);
                _layerNode = texture;
            }
        }

    }
}
