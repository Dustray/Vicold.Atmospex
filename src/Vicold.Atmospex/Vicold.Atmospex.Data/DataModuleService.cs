using Vicold.Atmospex.Data.DataCenter;
using System.IO;
using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Data;
public class DataModuleService : IDataModuleService
{
    private static IAppService? _appService;
    public static DataModuleService? Current
    {
        get; private set;
    }

    public DataModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
    }


    public ProductKeeper Productor { get; } = new(Path.GetFullPath("."));

    public void Initialize()
    {
    }

    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }
}
