using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Data.DataCenter;

namespace Vicold.Atmospex.Data;
public interface IDataModuleService : IModuleService
{
    // 接口成员
    ProductKeeper Productor
    {
        get;
    }
}
