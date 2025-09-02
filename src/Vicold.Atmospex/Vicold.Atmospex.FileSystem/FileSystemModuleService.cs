using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.FileSystem;

public class FileSystemModuleService : IFileSystemModuleService
{
    private static IAppService? _appService;

    public static FileSystemModuleService? Current
    {
        get; private set;
    }

    public FileSystemModuleService(IAppService appService)
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