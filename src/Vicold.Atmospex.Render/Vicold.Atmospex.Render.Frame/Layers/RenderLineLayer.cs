using Evergine.Common.Graphics;
using Evergine.Framework.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Earth.Tool;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Render.Frame.Nodes;

namespace Vicold.Atmospex.Render.Frame.Layers
{
    public class RenderLineLayer : LineLayer, IRenderLayer
    {
        public RenderLineLayer(IVectorDataProvider provider, string id) : base(provider, id)
        {
            LayerDescription = new()
            {
                RenderState = new RenderStateDescription()
                {
                    RasterizerState = new RasterizerStateDescription()
                    {
                        CullMode = CullMode.Back,
                        FillMode = FillMode.Wireframe,
                    },
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.ReadWrite,
                },
                Order = 0,
                SortMode = SortMode.FrontToBack,
            };
        }

        public RenderLayerDescription LayerDescription { get; private set; }

        protected override ILayerNode? CreateLinesNode(string ID, IVectorDataProvider provider, IProjection prj)
        {
            var vectorData = provider.GetData();
            if (vectorData is LineData lineData)
            {
                var lines = LineConverterTool.ToVectorLines(lineData, prj);
                var linesNode = new RenderLinesNode(lines)
                {
                    ID = ID
                };
                return linesNode;
            }

            return null;
        }

        public void Draw(EntityManager entityManager)
        {
            if (_layerNode is IRenderNode node)
            {
                node.Draw(entityManager, LayerDescription);
            }
        }
    }
}
