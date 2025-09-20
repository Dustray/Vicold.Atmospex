using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Provider.NetCDF;
using Vicold.Atmospex.FileSystem;
using Vicold.Atmospex.Layer;

namespace Vicold.Atmospex.Core.Bindings;
internal interface IBinding
{
    FileHost? TryGetFileHost(string filePath);
    ILayer? CreateLayer(FileHost fileHost);
}