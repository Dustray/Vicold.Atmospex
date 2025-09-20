using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Layer;

namespace Vicold.Atmospex.Core;

public interface ICoreModuleService : IModuleService
{
    // 接口成员
    Action? OnViewStart
    {
        get; set;
    }

    Func<IGridDataProvider, GridLayer>? BindingGridLayer
    {
        get; set;
    }

    Task AddDataAsync(string path);

}