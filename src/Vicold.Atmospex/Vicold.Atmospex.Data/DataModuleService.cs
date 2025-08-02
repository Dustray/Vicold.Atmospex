using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.DataCenter;

namespace Vicold.Atmospex.Data;
public class DataModuleService : IDataModuleService
{
    public DataModuleService()
    {
    }

    public static DataModuleService? Current
    {
        get; private set;
    }

    internal ProductKeeper Productor { get; } = new(Path.GetFullPath("."));

    public void Initialize()
    {
        Current = this;
    }


}
