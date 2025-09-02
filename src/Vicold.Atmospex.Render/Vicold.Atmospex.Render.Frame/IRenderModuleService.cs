using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Layer;

namespace Vicold.Atmospex.Render.Frame;

public interface IRenderModuleService : IModuleService
{
    // 接口成员
    void Bind(ILayerModuleService layerModuleService);
}