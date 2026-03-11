using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Godot.Services;

public class AppService : IAppService
{
    public void Initialize()
    {
        // 初始化应用服务
    }

    public T? GetService<T>() where T : class
    {
        return Vicold.Atmospex.Godot.App.GetService<T>();
    }
}
