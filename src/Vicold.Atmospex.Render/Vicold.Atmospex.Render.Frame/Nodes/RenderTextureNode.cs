using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Graphics.Materials;
using Evergine.Framework.Managers;
using Evergine.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Render.Frame.Meshes;

namespace Vicold.Atmospex.Render.Frame.Nodes
{
    public class RenderTextureNode : TextureNode, IRenderNode
    {
        private readonly GraphicsContext graphicsContext;

        public RenderTextureNode(GraphicsContext? graphicsContext = null)
        {
            this.graphicsContext = graphicsContext ?? Application.Current.Container.Resolve<GraphicsContext>();
        }

        public Texture? TImage
        {
            get; set;
        }

        public override void SetLevel(int zIndex)
        {
            // 设置纹理层级，如果需要
        }

        internal void ResetImage(Texture newImage)
        {
            // 释放旧纹理资源
            if (TImage != null)
            {
                TImage.Dispose();
            }

            // 设置新纹理
            TImage = newImage;
        }

        // 重载ResetImage方法，接受图像数据更新现有纹理
        internal void ResetImage(float[] imageData)
        {
            if (TImage == null)
            {
                throw new InvalidOperationException("Cannot update texture: TImage is null");
            }

            // 使用GraphicsContext更新纹理数据
            graphicsContext.UpdateTextureData(TImage, imageData);
        }

        public override void Dispose()
        {
            if (TImage != null)
            {
                TImage.Dispose();
                TImage = null;
            }
        }

        public void Draw(EntityManager entityManager, RenderLayerDescription layerDescription)
        {
            var assetsService = Application.Current.Container.Resolve<AssetsService>();

            // Load the standard effect
            Effect standardEffect = assetsService.Load<Effect>(EvergineContent.Effects.LineEffect);

            // Create a material using the custom RenderLayer
            var material = new Material(standardEffect)
            {
                LayerDescription = layerDescription,
            };
            material.SetTexture(TImage, 0);

            // Apply the material to an entity
            Entity primitive = new Entity()
                .AddComponent(new Transform3D())
                .AddComponent(new MaterialComponent() { Material = material })
                .AddComponent(new TeapotMesh())
                .AddComponent(new MeshRenderer());
            entityManager.Add(primitive);
        }
    }
}
