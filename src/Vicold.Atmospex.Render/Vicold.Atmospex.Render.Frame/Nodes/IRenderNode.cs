using Evergine.Common.Graphics;
using Evergine.Framework.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Render.Frame.Nodes
{
    internal interface IRenderNode
    {
        public bool IsTileEnabled { get; set; }

        void Draw(EntityManager entityManager, RenderLayerDescription layerDescription);
    }
}
