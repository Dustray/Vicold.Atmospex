using Evergine.Common.Graphics;
using Evergine.Framework.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Render.Frame.Layers
{
    internal interface IRenderLayer
    {
        RenderLayerDescription LayerDescription { get; }

        void Draw(EntityManager entityManager);
    }
}
