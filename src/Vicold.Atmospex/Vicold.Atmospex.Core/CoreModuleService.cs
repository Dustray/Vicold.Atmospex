using Vicold.Atmospex.Configration;
using Vicold.Atmospex.Core.Bindings;
using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Layer;

namespace Vicold.Atmospex.Core;

public class CoreModuleService : ICoreModuleService
{
    private static IAppService? _appService;
    private readonly IList<IBinding> bindings = [];
    private readonly ILayerManager? _layerManager;

    public static CoreModuleService? Current
    {
        get; private set;
    }

    public CoreModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
        _layerManager = appService.GetService<ILayerModuleService>()?.LayerManager;
        bindings.Add(new NetCDFBinding());
        bindings.Add(new RMBinding());
    }

    public void Initialize()
    {
        _appService?.GetService<IConfigModuleService>()?.Init(new BootConfig() { WorkSpaceDebug = "J:\\Example\\RMIAS\\dist" });
    }

    public Action? OnViewStart
    {
        get; set;
    }
    public Func<IGridDataProvider, GridLayer>? BindingGridLayer
    {
        get; set;
    }

    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }

    public Task AddDataAsync(string path)
    {
        if (_layerManager is null)
        {
            return Task.CompletedTask;
        }

        return Task.Run(() =>
        {
            foreach (var binding in bindings)
            {
                var fileHost = binding.TryGetFileHost(path);
                //Godot.GD.Print(path);
                if (fileHost != null)
                {
                    var layer = binding.CreateLayer(fileHost);
                    if (layer is { })
                    {
                        _layerManager.AddLayer(layer);
                    }
                }
            }
        });
    }
}