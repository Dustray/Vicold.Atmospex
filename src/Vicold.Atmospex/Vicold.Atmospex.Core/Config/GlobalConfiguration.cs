using Vicold.Atmospex.Core.Bus;
using Vicold.Atmospex.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vicold.Atmospex.Configration;

namespace Vicold.Atmospex.Core.Config
{
    class GlobalConfiguration : IGlobalConfiguration
    {

        public void Init(BootConfig bootConfig)
        {
#if DEBUG
            WorkSpace = bootConfig.WorkSpaceDebug;
#else
            WorkSpace = bootConfig.WorkSpace;
#endif

            StyleHome = Path.Combine(WorkSpace, "data/style");
            PaletteHome = Path.Combine(StyleHome, "palette");
        }


        public string WorkSpace { get; private set; }

        public string StyleHome { get; private set; }

        public string PaletteHome { get; private set; }
    }
}
