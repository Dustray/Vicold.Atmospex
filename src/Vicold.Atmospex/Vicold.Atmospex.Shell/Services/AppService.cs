using Microsoft.Extensions.DependencyInjection;
using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Shell.Services;

public class AppService : IAppService
{
    private readonly IServiceProvider _serviceProvider;

    public AppService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T? GetService<T>() where T : class
    {
        return _serviceProvider.GetService<T>();
    }
}