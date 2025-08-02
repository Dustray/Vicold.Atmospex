using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Render.Frame;

namespace Vicold.Atmospex.Layer.Tool
{
    public static class LayerExtractor
    {
        public static ILayerNode ExtractLayerNode(ILayer layer)
        {
            if (layer is Layer mLayer)
            {
                return mLayer._layerNode;
            }

            return null;
        }
    }
}
