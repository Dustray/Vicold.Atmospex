using Evergine.Common.Graphics;
using Evergine.Components.Cameras;
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
            //ettManager.Remove(camera);

            //ettManager.Add(new MyCamera("MyCamera"));
            //Entity cameraEntity = new Entity()
            // .AddComponent(new Transform3D())
            // .AddComponent(new Camera3D()
            // {
            //     BackgroundColor = Color.CornflowerBlue,
            // });
            //this.Managers.EntityManager.Add(cameraEntity);

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
            //trans.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi);// 无需旋转，默认面向 Z+
            //var lineContainer = new Entity()
            //               .AddComponent(trans)
            //               .AddComponent(lineMesh)
            //               .AddComponent(new LineMeshRenderer3D());
            //this.Managers.EntityManager.Add(lineContainer);



            var assetsService = Application.Current.Container.Resolve<AssetsService>();

            // Create a custom RenderLayer with specified render states
            RenderLayerDescription customLayer = new RenderLayerDescription()
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

            // Load the standard effect
            Effect standardEffect = assetsService.Load<Effect>(EvergineContent.Effects.StandardEffect);

            // Create a material using the custom RenderLayer
            Material material = new Material(standardEffect)
            {
                LayerDescription = customLayer
            };

            // Apply the material to an entity
            Entity primitive = new Entity()
                .AddComponent(new Transform3D())
                .AddComponent(new MaterialComponent() { Material = material })
                .AddComponent(new TeapotMesh())
                .AddComponent(new MeshRenderer());

            this.Managers.EntityManager.Add(primitive);
        }
    }
}


