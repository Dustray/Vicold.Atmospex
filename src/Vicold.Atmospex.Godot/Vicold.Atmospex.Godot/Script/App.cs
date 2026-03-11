using Godot;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Shell.Core.Contracts.Services;
using Vicold.Atmospex.Shell.Core.Services;

namespace Vicold.Atmospex.Godot;

public static class App
{
	// The .NET Generic Host provides dependency injection, configuration, logging, and other services.
	public static IHost? Host { get; private set; }

	public static T GetService<T>()
		where T : class
	{
		if (Host?.Services?.GetService(typeof(T)) is not T service)
		{
			throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.cs.");
		}

		return service;
	}

	public static void Initialize()
	{
		// Initialize the Host
		Host = Microsoft.Extensions.Hosting.Host.
		CreateDefaultBuilder().
		UseContentRoot(AppContext.BaseDirectory).
		ConfigureServices((context, services) =>
		{
			// Core Services
			services.AddSingleton<IFileService, FileService>();
			services.AddSingleton<IAppService, Vicold.Atmospex.Godot.Services.AppService>();

			// Module Services
			services.AddSingleton<IDataModuleService, DataModuleService>();
			services.AddSingleton<IDataHubService, DataHubService>();
			services.AddSingleton<IEarthModuleService, EarthModuleService>();
			services.AddSingleton<ILayerModuleService, LayerModuleService>();
			services.AddSingleton<Vicold.Atmospex.Algorithm.IAlgorithmModuleService, Vicold.Atmospex.Algorithm.AlgorithmModuleService>();
			services.AddSingleton<Vicold.Atmospex.Configration.IConfigModuleService, Vicold.Atmospex.Configration.ConfigModuleService>();
			services.AddSingleton<Vicold.Atmospex.Core.ICoreModuleService, Vicold.Atmospex.Core.CoreModuleService>();
			services.AddSingleton<Vicold.Atmospex.FileSystem.IFileSystemModuleService, Vicold.Atmospex.FileSystem.FileSystemModuleService>();
			services.AddSingleton<Vicold.Atmospex.Style.IStyleModuleService, Vicold.Atmospex.Style.StyleModuleService>();
			services.AddSingleton<Vicold.Atmospex.Godot.Frame.IRenderModuleService, Vicold.Atmospex.Godot.Frame.RenderModuleService>();
			services.AddSingleton<Vicold.Atmospex.Godot.Frame.Services.IInteractionService, Vicold.Atmospex.Godot.Frame.Services.InteractionService>();
		}).
		Build();

		// Initialize services
		Host.StartAsync();

		// Initialize modules
		App.GetService<Vicold.Atmospex.Configration.IConfigModuleService>().Initialize();
		App.GetService<IDataModuleService>().Initialize();
		var earth = App.GetService<IEarthModuleService>();
		earth.WorldScale = 1000;
		earth.Initialize();
		earth.ChangeProjection(Vicold.Atmospex.Earth.Projection.ProjectionType.CloseToReal);

		App.GetService<ILayerModuleService>().Initialize();
		App.GetService<Vicold.Atmospex.Core.ICoreModuleService>().Initialize();
		App.GetService<Vicold.Atmospex.FileSystem.IFileSystemModuleService>().Initialize();
		App.GetService<Vicold.Atmospex.Style.IStyleModuleService>().Initialize();
		App.GetService<Vicold.Atmospex.Godot.Frame.IRenderModuleService>().Initialize();

		AfterInit();
	}

	private static void AfterInit()
	{
		App.GetService<Vicold.Atmospex.Core.ICoreModuleService>().OnViewStart = () =>
		{
			System.Threading.Tasks.Task.Run(() =>
			{
				App.GetService<Vicold.Atmospex.Godot.Frame.IRenderModuleService>().SetLaunchGeoPosition(105f, 35f, 3f);

				var mapHolder = new Vicold.Atmospex.Earth.Map.MapHolder();

				//var worldLayerPolygon = new RenderLineLayer(mapHolder.WorldPolygonProvider, "rmias_world_polygon");
				var geoGridLayer = new Vicold.Atmospex.Godot.Frame.Layers.RenderLineLayer(mapHolder.GeoGridProvider, "rmias_geo_line") { ZIndex = 6 };
				var geoFontLayer = new Vicold.Atmospex.Godot.Frame.Layers.RenderFontLayer(mapHolder.GeoFontProvider, "rmias_geo_value") { ZIndex = 20 };
				var worldLayerLine = new Vicold.Atmospex.Godot.Frame.Layers.RenderLineLayer(mapHolder.WorldLineProvider, "rmias_world_line") { ZIndex = 7 };
				var chinaCoastalLayer = new Vicold.Atmospex.Godot.Frame.Layers.RenderLineLayer(mapHolder.ChinaCoastalProvider, "rmias_china_line") { ZIndex = 9 };
				var chinaProvinceLayer = new Vicold.Atmospex.Godot.Frame.Layers.RenderLineLayer(mapHolder.ChinaProvinceProvider, "rmias_province_line") { ZIndex = 8 };

				var manager = App.GetService<ILayerModuleService>().LayerManager;
				manager.AddLayer(geoGridLayer);
				manager.AddLayer(geoFontLayer);
				manager.AddLayer(worldLayerLine);
				manager.AddLayer(chinaCoastalLayer);
				manager.AddLayer(chinaProvinceLayer);
			});
		};
	}
}
