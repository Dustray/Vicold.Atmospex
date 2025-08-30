using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evergine.Common.Graphics;
using Evergine.Framework;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Render.Frame.Meshes;

namespace Vicold.Atmospex.Render.Frame.Nodes
{
    public class RenderTextureNode : TextureNode
    {
        private GraphicsContext graphicsContext;

        public RenderTextureNode(GraphicsContext graphicsContext = null)
        {
            this.graphicsContext = graphicsContext ?? Application.Current.Container.Resolve<GraphicsContext>();
        }

        public Texture TImage
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

        public void Dispose()
        {
            if (TImage != null)
            {
                TImage.Dispose();
                TImage = null;
            }
        }
    }
}
