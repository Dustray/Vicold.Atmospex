using Evergine.Common.Graphics;
using Evergine.Components.Fonts;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Graphics.Materials;
using Evergine.Framework.Managers;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Layer.Node;

namespace Vicold.Atmospex.Render.Frame.Nodes
{
    internal class RenderFontNode : TextureNode, IRenderNode
    {
        private readonly VectorData _fontData;
        private readonly RenderLayerDescription _renderLayer;
        private Entity? _fontEntity;
        private IList<VectorFont> _vectorFonts = [];

        public RenderFontNode(VectorData fontData, RenderLayerDescription renderLayer)
        {
            _fontData = fontData;
            _renderLayer = renderLayer;
            _fontEntity = null;
        }

        /// <summary>
        /// 设置字体数据
        /// </summary>
        /// <param name="fonts">字体列表</param>
        public void SetFonts(IList<VectorFont> fonts)
        {
            _vectorFonts = fonts;
            // 当字体数据更新时，重置字体实体以便重新创建
            if (_fontEntity != null)
            {
                // 释放旧的字体实体资源
                foreach (var c in _fontEntity.ChildEntities)
                {
                    _fontEntity.DetachChild(c);
                }

                _fontEntity = null;
            }
        }

        public override void SetLevel(int zIndex)
        {
            ZIndex = zIndex;
        }

        public int ZIndex { get; set; }

        public void Draw(EntityManager entityManager, RenderLayerDescription layerDescription)
        {
            if (_fontEntity == null)
            {
                // 创建容器实体
                _fontEntity = new Entity();
                entityManager.Add(_fontEntity);

                // 检查是否有字体数据
                if (_vectorFonts != null && _vectorFonts.Count > 0)
                {
                    // 直接使用VectorFonts创建文本实体
                    CreateTextEntitiesFromVectorFonts(_vectorFonts, _fontEntity, layerDescription);
                }
            }
        }

        /// <summary>
        /// 从VectorFont列表创建文本实体
        /// </summary>
        /// <param name="vectorFonts">字体列表</param>
        /// <param name="parentEntity">父实体</param>
        /// <param name="layerDescription">渲染层描述</param>
        private void CreateTextEntitiesFromVectorFonts(IList<VectorFont> vectorFonts, Entity parentEntity, RenderLayerDescription layerDescription)
        {
            var assetsService = Application.Current.Container.Resolve<AssetsService>();
            var defaultFont = assetsService.Load<Font>(DefaultResourcesIDs.DefaultFontID);

            // 遍历所有字体元素
            foreach (var vectorFont in vectorFonts)
            {
                var textMesh = new Text3DMesh()
                {
                    Font = defaultFont,
                    Text = vectorFont.Font,
                    Origin = new Vector2(0f, 0f), // Center the text
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    ScaleFactor = vectorFont.FontSize/300, // Adjust the scale factor as needed
                    Size = new Vector2(2f, 1f),
                    Wrapping = false,
                    Color = new Color(
                        vectorFont.FontColor.R,
                        vectorFont.FontColor.G,
                        vectorFont.FontColor.B,
                        vectorFont.FontColor.A
                    ),
                };

                // 获取位置信息
                Vector3 position = new Vector3(vectorFont.Position.X- textMesh.Size.X/2, vectorFont.Position.Y+ textMesh.Size.Y/2, 0);
                var renderer = new Text3DRenderer() { DebugMode = false };
                var textTrans = new Transform3D
                {
                    Position = position,
                };
                // 创建立方体作为文本占位符
                Entity textEntity = new Entity()
                    .AddComponent(textTrans)
                    .AddComponent(textMesh)
                    .AddComponent(renderer);
                // 添加到父实体
                parentEntity.AddChild(textEntity);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_fontEntity != null)
            {
                // 释放旧的字体实体资源
                foreach (var c in _fontEntity.ChildEntities)
                {
                    _fontEntity.DetachChild(c);
                }

                _fontEntity = null;
            }
        }
    }
}