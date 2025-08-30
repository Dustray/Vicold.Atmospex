using Evergine.Common.Graphics;
using Evergine.Framework.Runtimes;
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
    internal class RenderLinesNode : LinesNode
    {
        private GenericLineMesh _lineMesh =new ();
        //private Texture? _context; 

        public RenderLinesNode(VectorLine[] lines) : base(lines) { }
        public override void SetLevel(int zIndex)
        {
        }

        public async  void InitMesh()
        {

        }
    }
}
