using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Graphics.Materials;
using Evergine.Framework.Managers;
using Evergine.Framework.Physics3D;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Render.Frame.Meshes;
using static Evergine.Components.Graphics3D.PlaneMesh;

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

        public bool IsTileEnabled { get; set; } = true;

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

            // 加载Effect
            Effect standardEffect = assetsService.Load<Effect>(EvergineContent.Effects.StandardEffect);

            // 创建StandardMaterial并设置属性
            StandardMaterial standardMaterial = new StandardMaterial(standardEffect);

            // 设置渲染层
            standardMaterial.LayerDescription = layerDescription;

            // 设置光照模式 - 禁用光照
            standardMaterial.LightingEnabled = false;

            // 应用纹理或设置基础颜色
            if (TImage != null)
            {
                // 使用TImage作为纹理
                standardMaterial.BaseColorTexture = TImage;
                // 如果使用纹理，可以设置透明通道（如果需要）
            }
            else
            {
                // 如果没有纹理，使用红色作为基础颜色
                standardMaterial.BaseColor = Color.Red;
            }

            // 根据TextureNode基类中的属性设置位置和大小
            var tectTrans = new Transform3D
            {
                Position = new Vector3(0, 0, 0),
                //Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi) // 无需旋转，默认面向 Z+
            };
            // 创建PlaneMesh实体，使用WorldWidth和WorldHeight设置大小
            Entity planeEntity = new Entity()
                .AddComponent(tectTrans)
                .AddComponent(new PlaneMesh()
                {
                    Width = WorldWidth,
                    Height = WorldHeight,
                    TwoSides = false,
                    PlaneNormal = NormalAxis.ZPositive,
                    VMirror = true,
                    //UMirror = true,
                })
                .AddComponent(new MaterialComponent() { Material = standardMaterial.Material })
                .AddComponent(new MeshRenderer());

            // 添加到场景
            entityManager.Add(planeEntity);
        }
    }
}
