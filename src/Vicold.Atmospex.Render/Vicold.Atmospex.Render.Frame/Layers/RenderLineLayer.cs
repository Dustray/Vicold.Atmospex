using Evergine.Common.Graphics;
using Evergine.Framework.Managers;
using System;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Earth.Tool;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Render.Frame.Nodes;
using Vicold.Atmospex.Style;

namespace Vicold.Atmospex.Render.Frame.Layers
{
    public class RenderLineLayer : LineLayer, IRenderLayer
    {
        private RenderType _renderType;

        public RenderLineLayer(IVectorDataProvider provider, string id, RenderType renderType = RenderType.Contour, bool cutLineToTile = true) : base(provider, id)
        {
            _renderType = renderType;
            CutLineToTile = cutLineToTile;
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

        public bool CutLineToTile { get; }

        protected override ILayerNode? CreateLinesNode(string ID, IVectorDataProvider provider, IProjection prj)
        {
            var vectorData = provider.GetData();
            if (_renderType == RenderType.Contour)
            {

                if (vectorData is LineData lineData)
                {
                    var lines = LineConverterTool.ToVectorLines(lineData, prj);
                    var linesNode = new RenderLinesNode(lines, LayerDescription)
                    {
                        ID = ID
                    };
                    return linesNode;
                }
            }
            else if (_renderType == RenderType.Polygon)
            {
                if (vectorData is LineData polygonData)
                {
                    var lines = LineConverterTool.ToVectorLines(polygonData, prj);
                    var linesNode = new RenderPolygonsNode(lines, LayerDescription)
                    {
                        ID = ID
                    };
                    return linesNode;
                }
            }

            return null;
        }

        public void Draw(EntityManager entityManager)
        {
            if (_layerNode is IRenderNode node)
            {
                node.IsTileEnabled = CutLineToTile;
                node.Draw(entityManager, LayerDescription);
            }
        }
    }
}
