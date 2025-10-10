using Evergine.Common.Graphics;
using Evergine.Common.Graphics.VertexFormats;
using Evergine.Components.Cameras;
using Evergine.Components.Environment;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Graphics.Materials;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System.Runtime.CompilerServices;
using Vicold.Atmospex.Render.Components;

namespace Vicold.Atmospex.Render
{
    public class MyScene : Scene
    {
        public override void RegisterManagers()
        {
            base.RegisterManagers();

            this.Managers.AddManager(new global::Evergine.Bullet.BulletPhysicManager3D());

        }

        protected override void Start()
        {
            base.Start();
        }

        public void CreateSimpleFilledTriangle(GraphicsContext gfx, AssetsService assets, RenderLayerDescription layerDesc)
        {
            // 手工顶点：一个简单三角形
            var positions = new Vector3[]
            {
        new Vector3(0f, 0f, 0f),
        new Vector3(200f, 0f, 0f),
        new Vector3(100f, 150f, 0f)
            };

            var color = new Color(255, 100, 100, 255);

            var verts = new VertexPositionColor[3];
            for (int i = 0; i < 3; i++)
            {
                verts[i].Position = positions[i];
                verts[i].Color = color;
            }

            ushort[] indices = new ushort[] { 0, 1, 2 };

            // vertex / index buffer desc
            var vbDesc = new BufferDescription((uint)(verts.Length * Unsafe.SizeOf<VertexPositionColor>()),
                                               BufferFlags.VertexBuffer | BufferFlags.ShaderResource,
                                               ResourceUsage.Default);
            var ibDesc = new BufferDescription((uint)(indices.Length * sizeof(ushort)),
                                               BufferFlags.IndexBuffer,
                                               ResourceUsage.Default);

            var vBuffer = gfx.Factory.CreateBuffer(verts, ref vbDesc);
            var iBuffer = gfx.Factory.CreateBuffer(indices, ref ibDesc);

            var vb = new VertexBuffer(vBuffer, VertexPositionColor.VertexFormat);
            var ib = new IndexBuffer(iBuffer);

            var mesh = new Mesh(new[] { vb }, ib, PrimitiveTopology.TriangleList)
            {
                BoundingBox = new BoundingBox(new Vector3(0), new Vector3(200, 200, 0)),
                AllowBatching = true
            };

            // 用 StandardEffect 创建一个简单材质（无光照，使用统一颜色）
            var effect = assets.Load<Effect>(EvergineContent.Effects.StandardEffect);
            var mat = new StandardMaterial(effect)
            {
                VertexColorEnabled = false, // 先不用顶点色，改用材质固有颜色测试
                LightingEnabled = false,
                IBLEnabled = false,
                LayerDescription = layerDesc,
                Alpha = 1.0f
            };

            // 设置材质主色（示例 API，按你项目调整）
            // 如果 StandardMaterial 暴露 DiffuseColor/Albedo 可直接设置：
            // mat.DiffuseColor = new Color4(1, 0.4f, 0.4f, 1);

            var model = new Model("TestTri", mesh);
            var entity = model.InstantiateModelHierarchy("TestTriEntity", assets);
            entity.AddComponent(new MaterialComponent() { Material = mat.Material });
            entity.IsEnabled = true;

            // 把 entity 添加到当前场景（按你的场景管理方式）
            this.Managers.EntityManager.Add(entity);
        }
        protected override void CreateScene()
        {
            var ettManager = Managers.EntityManager;
            var camera = ettManager.Find("Camera");
            camera.RemoveComponent<FreeCamera3D>();
            camera.AddComponent(new EverFreeCamera3D());

            // 设置相机的背景颜色 - 这是设置渲染环境默认颜色的主要方法
            var camera3D = camera.FindComponent<Camera3D>();
            if (camera3D != null)
            {
                // 设置背景颜色为深蓝色
                camera3D.BackgroundColor = new Color(100, 149, 237);
            }

            // 如果需要，也可以通过创建新相机实体来设置默认颜色
            //Entity customCamera = new Entity()
            // .AddComponent(new Transform3D())
            // .AddComponent(new Camera3D()
            // {
            //     BackgroundColor = Color.CornflowerBlue,
            //     // 其他相机属性...
            // });
            //this.Managers.EntityManager.Add(customCamera);

            //var dummyEntity = new Entity()
            //.AddComponent(new Transform3D())
            //.AddComponent(new EverBatchLine());
            //this.Managers.EntityManager.Add(dummyEntity);

            //var lineMesh = new LineMesh();
            ////lineMesh.IsCameraAligned = true;
            //lineMesh.LineType = LineType.LineStrip;
            //lineMesh.LinePoints =
            //[
            //   new ()
            //   {
            //       Position = new Vector3(3.25f,5,1),
            //       Color = Color.Tomato,
            //       Thickness = 0.1f,
            //   },
            //   new ()
            //   {
            //       Position = new Vector3(3.1f,4.55f,1),
            //       Color = Color.Green,
            //       Thickness = 0.1f,
            //   },
            //   new ()
            //   {
            //       Position = new Vector3(2.95f,5f,1),
            //       Color = Color.Blue,
            //       Thickness = 0.1f,
            //   },
            //];

            //var trans = new Transform3D();
            //trans.Position = new Vector3(0,0, 1.2f).ToChangeOverlook();
            ////trans.ToRotateY(180);
            //trans.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi);// ������ת��Ĭ������ Z+
            //var lineContainer = new Entity()
            //               .AddComponent(trans)
            //               .AddComponent(lineMesh)
            //               .AddComponent(new LineMeshRenderer3D());
            //this.Managers.EntityManager.Add(lineContainer);



            //var assetsService = Application.Current.Container.Resolve<AssetsService>();

            //// Create a custom RenderLayer with specified render states
            RenderLayerDescription customLayer = new()
            {
                RenderState = new RenderStateDescription()
                {
                    RasterizerState = new RasterizerStateDescription()
                    {
                        CullMode = CullMode.None,
                        FillMode = FillMode.Solid,
                    },
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.ReadWrite,
                },
                Order = 0,
                SortMode = SortMode.FrontToBack,
            };

            //// Load the standard effect
            //Effect standardEffect = assetsService.Load<Effect>(EvergineContent.Effects.LineEffect);

            //// Create a material using the custom RenderLayer
            //Material material = new Material(standardEffect)
            //{
            //    LayerDescription = customLayer
            //};

            //// Apply the material to an entity
            //Entity primitive = new Entity()
            //    .AddComponent(new Transform3D())
            //    .AddComponent(new MaterialComponent() { Material = material })
            //    .AddComponent(new TeapotMesh())
            //    .AddComponent(new MeshRenderer());
            //this.Managers.EntityManager.Add(primitive);

            var sun = new Entity()
      .AddComponent(new Transform3D() { LocalRotation = new Vector3(-2, 0, 90) }) // Add some rotation to the light...
      .AddComponent(new PhotometricDirectionalLight())
      .AddComponent(new SunComponent());

            this.Managers.EntityManager.Add(sun);

            //      // Create the sphere sky dome
            //      var skyDome = new Entity()
            //          .AddComponent(new Transform3D() { LocalRotation = new Vector3(0, 0, 90) })
            //          .AddComponent(new MaterialComponent())
            //          .AddComponent(new SphereMesh())
            //          .AddComponent(new MeshRenderer())
            //          .AddComponent(new AtmosphereController());

            //      this.Managers.EntityManager.Add(skyDome);



            //var graphicsContext = Application.Current.Container.Resolve<GraphicsContext>();
            //var assetsService = Application.Current.Container.Resolve<AssetsService>();
            //CreateSimpleFilledTriangle(graphicsContext, assetsService, customLayer);
        }
    }
}


