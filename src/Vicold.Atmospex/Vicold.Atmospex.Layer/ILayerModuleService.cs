using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Layer;

public interface ILayerModuleService : IModuleService
{
    // 接口成员
    ILayerManager LayerManager
    {
        get;
    }
}
