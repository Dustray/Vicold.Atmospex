using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Style;

public class StyleModuleService : IStyleModuleService
{
    private static IAppService? _appService;

    public static StyleModuleService? Current
    {
        get; private set;
    }

    public StyleModuleService(IAppService appService)
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