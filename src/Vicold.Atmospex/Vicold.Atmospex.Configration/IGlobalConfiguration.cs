using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Configration
{
    public interface IGlobalConfiguration
    {

        string WorkSpace { get; }

        string StyleHome { get; }

        string PaletteHome { get; }
    }
}
