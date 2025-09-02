using Vicold.Atmospex.Configration;
using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Core;

public class CoreModuleService : ICoreModuleService
{
    private static IAppService? _appService;

    public static CoreModuleService? Current
    {
        get; private set;
    }

    public CoreModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
    }

    public void Initialize()
    {
        _appService?.GetService<IConfigModuleService>()?.Init(new BootConfig() { WorkSpaceDebug = "J:\\Example\\RMIAS\\dist" });
    }

    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }
}