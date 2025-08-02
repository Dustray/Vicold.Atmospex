using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.DataService.Provider
{
    public interface IData : ICloneable, IDisposable
    {

    }
    public interface IGridData : IData
    {
    }
}