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
using System.Linq;
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

        public bool IsTileEnabled { get; set; } = true;

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
                    Origin = new Vector2(vectorFont.Pivot.X, vectorFont.Pivot.Y), // Center the text
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    ScaleFactor = vectorFont.FontSize / 800, // Adjust the scale factor as needed
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
                Vector3 position = new Vector3(vectorFont.Position.X , vectorFont.Position.Y, 0);
                var renderer = new Text3DRenderer() { DebugMode = false };
                var textTrans = new Transform3D
                {
                    LocalPosition = position,
                    LocalScale = new Vector3(1, 1, 1),
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

        internal void UpdateScale(float scale)
        {
            // 检查字体实体是否存在
            if (_fontEntity == null || _fontEntity.ChildEntities.Count() == 0)
                return;

            // 遍历所有文本子实体
            foreach (var childEntity in _fontEntity.ChildEntities)
            {
                // 获取Transform3D组件
                var textTrans = childEntity.FindComponent<Transform3D>();
                if (textTrans == null)
                    continue;

                // 计算缩放因子，使字体大小相对于屏幕保持不变
                // 当相机高度增加时，需要放大字体；当相机高度减小时，需要缩小字体
                float scaleFactor = scale; // 直接使用相机高度作为缩放因子
                
                // 添加边界保护，避免字体变得过大或过小
                scaleFactor = Math.Max(0.1f, Math.Min(10.0f, scaleFactor));

                // 应用调整后的缩放因子
                textTrans.LocalScale = new Vector3(scaleFactor, scaleFactor, 1.0f);
            }
        }
    }
}