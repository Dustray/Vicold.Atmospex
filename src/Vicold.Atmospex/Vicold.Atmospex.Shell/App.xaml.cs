using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Earth.Map;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Render.Frame;
using Vicold.Atmospex.Render.Frame.Layers;
using Vicold.Atmospex.Shell.Activation;
using Vicold.Atmospex.Shell.Contracts.Services;
using Vicold.Atmospex.Shell.Core.Contracts.Services;
using Vicold.Atmospex.Shell.Core.Services;
using Vicold.Atmospex.Shell.Helpers;
using Vicold.Atmospex.Shell.Models;
using Vicold.Atmospex.Shell.Notifications;
using Vicold.Atmospex.Shell.Services;
using Vicold.Atmospex.Shell.ViewModels;
using Vicold.Atmospex.Shell.Views;

namespace Vicold.Atmospex.Shell;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IAppService, AppService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            //Module Services
            services.AddSingleton<IDataModuleService, DataModuleService>();
            services.AddSingleton<IEarthModuleService, EarthModuleService>();
            services.AddSingleton<ILayerModuleService, LayerModuleService>();
            services.AddSingleton<Vicold.Atmospex.Algorithm.IAlgorithmModuleService, Vicold.Atmospex.Algorithm.AlgorithmModuleService>();
            services.AddSingleton<Vicold.Atmospex.Configration.IConfigModuleService, Vicold.Atmospex.Configration.ConfigModuleService>();
            services.AddSingleton<Vicold.Atmospex.Core.ICoreModuleService, Vicold.Atmospex.Core.CoreModuleService>();
            services.AddSingleton<Vicold.Atmospex.FileSystem.IFileSystemModuleService, Vicold.Atmospex.FileSystem.FileSystemModuleService>();
            services.AddSingleton<Vicold.Atmospex.Style.IStyleModuleService, Vicold.Atmospex.Style.StyleModuleService>();
            services.AddSingleton<Vicold.Atmospex.Render.Frame.IRenderModuleService, Vicold.Atmospex.Render.Frame.RenderModuleService>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();
        App.GetService<Vicold.Atmospex.Configration.IConfigModuleService>().Initialize();
        App.GetService<IDataModuleService>().Initialize();
        var earth = App.GetService<IEarthModuleService>();
        earth.WorldScale = 1000;
        earth.Initialize();
        earth.ChangeProjection(ProjectionType.CloseToReal);

        App.GetService<ILayerModuleService>().Initialize();
        App.GetService<Vicold.Atmospex.Core.ICoreModuleService>().Initialize();
        App.GetService<Vicold.Atmospex.FileSystem.IFileSystemModuleService>().Initialize();
        App.GetService<Vicold.Atmospex.Style.IStyleModuleService>().Initialize();
        App.GetService<Vicold.Atmospex.Render.Frame.IRenderModuleService>().Initialize();

        UnhandledException += App_UnhandledException;
        AfterInit();
    }

    private void AfterInit()
    {
        //App.GetService<Vicold.Atmospex.Render.Frame.IRenderModuleService>().Bind(App.GetService<ILayerModuleService>());
        App.GetService<Vicold.Atmospex.Core.ICoreModuleService>().OnViewStart = () =>
        {
            Task.Run(() =>
            {
                var mapHolder = new MapHolder();

                //var worldLayerPolygon = new RenderLineLayer(mapHolder.WorldPolygonProvider, "rmias_world_polygon");
                var geoGridLayer = new RenderLineLayer(mapHolder.GeoGridProvider, "rmias_geo_line", cutLineToTile: false) { ZIndex = 6 };
                var geoFontLayer = new RenderFontLayer(mapHolder.GeoFontProvider, "rmias_geo_value") { ZIndex = 20 };
                var worldLayerLine = new RenderLineLayer(mapHolder.WorldLineProvider, "rmias_world_line") { ZIndex = 7 };
                var chinaCoastalLayer = new RenderLineLayer(mapHolder.ChinaCoastalProvider, "rmias_china_line", Style.RenderType.Contour) { ZIndex = 9 };
                var chinaProvinceLayer = new RenderLineLayer(mapHolder.ChinaProvinceProvider, "rmias_province_line", Style.RenderType.Contour) { ZIndex = 8 };

                var manager = App.GetService<ILayerModuleService>().LayerManager;
                manager.AddLayer(geoGridLayer);
                manager.AddLayer(geoFontLayer);
                manager.AddLayer(worldLayerLine);
                manager.AddLayer(chinaCoastalLayer);
                manager.AddLayer(chinaProvinceLayer);
            });
        };
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        // 异步加载窗口位置和大小
        await AppWindowHelper.LoadWindowPlacementAsync(MainWindow);

        // 监听窗口关闭事件以保存位置和大小
        MainWindow.Closed += OnMainWindowClosed;

        App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);
    }

    private void OnMainWindowClosed(object sender, WindowEventArgs args)
    {
        // 保存窗口位置和大小（Fire and Forget模式，不等待完成）
        AppWindowHelper.SaveWindowPlacementAsync(MainWindow);
    }

}
