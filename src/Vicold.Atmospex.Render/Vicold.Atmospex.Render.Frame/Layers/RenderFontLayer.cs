using Evergine.Common.Graphics;
using Evergine.Framework;
using Evergine.Framework.Managers;
using System.Collections.Generic;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Render.Frame.Nodes;

namespace Vicold.Atmospex.Render.Frame.Layers
{
    public class RenderFontLayer : FontLayer, IRenderLayer
    {
        private readonly GraphicsContext _graphicsContext;

        public RenderFontLayer(IVectorDataProvider provider, string id)
            : base(provider, id)
        {
            _graphicsContext = Application.Current.Container.Resolve<GraphicsContext>();

            // 创建自定义渲染层描述
            LayerDescription = new()
            {
                RenderState = new RenderStateDescription()
                {
                    RasterizerState = new RasterizerStateDescription()
                    {
                        CullMode = CullMode.Back,
                        FillMode = FillMode.Solid,
                    },
                    BlendState = BlendStates.AlphaBlend,
                    DepthStencilState = DepthStencilStates.ReadWrite,
                },
                Order = 110, // 字体层通常在顶层
                SortMode = SortMode.FrontToBack,
            };
        }

        public RenderLayerDescription LayerDescription { get; private set; }

        protected override ILayerNode? CreateVectorsNode(string ID, IVectorDataProvider provider, IProjection proj)
        {
            var vectorData = provider.GetData();
            if (vectorData is VectorData vector)
            {
                IList<VectorFont> vectorFonts = new List<VectorFont>();
                var count = vector.Collection.Count;
                for (var i = 0; i < count; i++)
                {
                    var vectorElement = vector.Collection[i];
                    if (vectorElement.Type == VectorType.Font)
                    {
                        if (vectorElement is FontElement fontElement)
                        {
                            var position = fontElement.Position;
                            proj.Geo2Index(position.X, position.Y, out var x, out var y);
                            var offset = fontElement.Offset;
                            //proj.Geo2Index(offset.X, offset.Y, out var offsetX, out var offsetY);
                            vectorFonts.Add(new VectorFont()
                            {
                                Font = (string)fontElement.Data,
                                Position = new System.Numerics.Vector2((float)(x + offset.X), (float)(y + offset.Y)),
                                Pivot = new System.Numerics.Vector2(fontElement.Pivot.X, fontElement.Pivot.Y),
                                FontColor = fontElement.FontColor,
                                FontSize = fontElement.FontSize,
                            });
                        }
                    }
                    else if (vectorElement.Type == VectorType.Symbol)
                    {
                        // 处理符号类型元素（如果需要）
                    }
                }

                // 创建字体节点并设置字体数据
                var fontNode = new RenderFontNode(vector, LayerDescription)
                {
                    ID = ID
                };

                // 设置字体数据到节点
                fontNode.SetFonts(vectorFonts);

                return fontNode;
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