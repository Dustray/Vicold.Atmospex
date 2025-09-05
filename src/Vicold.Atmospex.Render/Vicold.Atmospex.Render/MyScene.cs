using Evergine.Common.Graphics;
using Evergine.Components.Cameras;
using Evergine.Components.Environment;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Services;
using Evergine.Mathematics;
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
            //RenderLayerDescription customLayer = new()
            //{
            //    RenderState = new RenderStateDescription()
            //    {
            //        RasterizerState = new RasterizerStateDescription()
            //        {
            //            CullMode = CullMode.Back,
            //            FillMode = FillMode.Wireframe,
            //        },
            //        BlendState = BlendStates.Opaque,
            //        DepthStencilState = DepthStencilStates.ReadWrite,
            //    },
            //    Order = 0,
            //    SortMode = SortMode.FrontToBack,
            //};

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
        }
    }
}


