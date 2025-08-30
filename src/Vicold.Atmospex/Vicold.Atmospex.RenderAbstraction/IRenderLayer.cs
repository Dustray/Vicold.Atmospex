using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.RenderAbstraction
{
    internal interface IRenderLayer : IDisposable
    {
        void Render();
    }
}
