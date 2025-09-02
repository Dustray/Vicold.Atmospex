using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Render;

public class RenderModuleService : IRenderModuleService
{
    private static IAppService? _appService;

    public static RenderModuleService? Current
    {
        get; private set;
    }

    public RenderModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
    }

    public void Initialize()
    {
    }

    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }
}