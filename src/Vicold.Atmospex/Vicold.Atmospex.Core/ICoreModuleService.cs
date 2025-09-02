using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Core;

public interface ICoreModuleService : IModuleService
{
    // 接口成员
    Action? OnViewStart
    {
        get; set;
    }
}