using Vicold.Atmospex.Earth.Projection;
using System;
using System.Collections.Generic;
using System.Text;
using Vicold.Atmospex.Data.Providers;

namespace Vicold.Atmospex.Layer
{
    public abstract class LineLayer : Layer
    {
        public LineLayer(IVectorDataProvider provider, string id)
        {
            DataProvider = provider;
            LayerZLevel = LayerLevel.High;
            ID = id;
        }

        protected abstract ILayerNode? CreateLinesNode(string ID, IVectorDataProvider provider, IProjection prj);

        public override void Render(IProjection projection)
        {
            base.Render(projection);

            if (DataProvider is not IVectorDataProvider provider)
            {
                throw new Exception("未指定Provider");
            }

            var texture = CreateLinesNode(ID, provider, projection);
            if (texture != null)
            {
                texture.SetLevel((int)LayerZLevel + ZIndex);
                _layerNode = texture;
            }
        }
    }
}
