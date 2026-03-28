using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Layer;

namespace Vicold.Atmospex.Godot.Frame
{
    public class VisionGate
    {
        public VisionGate()
        {
        }

        public Action<ILayerNode> OnNodeLoad { get; set; }

        public Action<ILayerNode> OnNodeRemove { get; set; }

        public Action<ILayerNode, bool> OnNodeVisibleChanged { get; set; }
    }
}
