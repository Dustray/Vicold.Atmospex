using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Data.Providers;
public interface IVectorDataProvider : IProvider
{
    IVectorData GetData();
}
