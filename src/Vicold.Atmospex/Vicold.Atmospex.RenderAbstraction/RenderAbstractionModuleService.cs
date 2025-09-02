using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Core.Core;

namespace Vicold.Atmospex.RenderAbstraction;

public class RenderAbstractionModuleService : IRenderAbstractionModuleService
{
    private static IAppService? _appService;

    public static RenderAbstractionModuleService? Current
    {
        get; private set;
    }

    public RenderAbstractionModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
    }

    public void Initialize()
    {
        // 初始化逻辑
    }

    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }
}