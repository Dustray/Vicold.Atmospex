using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Managers;
using Evergine.Framework.Runtimes;
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
    internal class RenderLinesNode : LinesNode, IRenderNode
    {
        private GenericLineMesh _lineMesh = new();
        //private Texture? _context; 

        public RenderLinesNode(VectorLine[] lines) : base(lines) {
        
        }
        public override void SetLevel(int zIndex)
        {
            ZIndex = zIndex;
        }

        public int ZIndex { get; set; }

        public void Draw(EntityManager entityManager, RenderLayerDescription layerDescription)
        {
            var assetsService = Application.Current.Container.Resolve<AssetsService>();

            // Load the standard effect
            Effect standardEffect = assetsService.Load<Effect>(EvergineContent.Effects.LineEffect);

            layerDescription.Order = ZIndex;
            // Create a material using the custom RenderLayer
            var material = new Material(standardEffect)
            {
                LayerDescription = layerDescription,
            };

            // Apply the material to an entity
            Entity primitive = new Entity()
                .AddComponent(new Transform3D())
                .AddComponent(new MaterialComponent() { Material = material })
                .AddComponent(_lineMesh)
                .AddComponent(new MeshRenderer());
            entityManager.Add(primitive);
        }
    }
}
