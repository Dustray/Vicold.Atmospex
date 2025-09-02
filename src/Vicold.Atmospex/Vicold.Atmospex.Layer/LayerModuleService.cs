using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Layer;
public class LayerModuleService : ILayerModuleService
{
    private static IAppService? _appService;

    public static LayerModuleService? Current
    {
        get; private set;
    }

    public LayerModuleService(IAppService appService)
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
