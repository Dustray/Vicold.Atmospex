using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Managers;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;
using Vicold.Atmospex.Core;
using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Render.Frame.Layers;
using Vicold.Atmospex.Render.Serviecs;

namespace Vicold.Atmospex.Render.Frame;

public class RenderModuleService : IRenderModuleService
{
    private static IAppService? _appService;
    private EntityManager? _entityManager;
    private MouseInteractionService? _mouseInteractionService;

    // 地球投影服务
    private readonly IEarthModuleService _earthService;

    // 相机
    private Camera? _currentCamera => _mouseInteractionService?.BindingCamera;

    public static RenderModuleService? Current
    {
        get; private set;
    }

    public RenderModuleService(IAppService appService, IEarthModuleService earthService)
    {
        _appService = appService;
        Current = this;

        // 初始化地球服务
        _earthService = earthService;
    }


    public ViewportInfo Viewport { get; } = new();


    public void Initialize()
    {
        var coreService = GetService<ICoreModuleService>();
        if (coreService is { })
        {
            coreService.BindingGridLayer = (provider) =>
            {
                var graphicsContext = Application.Current.Container.Resolve<GraphicsContext>();
                var layer = new RenderGridLayer(provider, graphicsContext);
                return layer;
            };
        }
    }

    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }

    public void Bind(ILayerModuleService layerModuleService)
    {
        layerModuleService.LayerManager.OnLayerAdded += (s, e) =>
        {
            if (e.Layer is IRenderLayer el && _entityManager is { })
            {
                el.Draw(_entityManager);
            }
        };
    }

    internal void BindEntityManager(EntityManager? entityManager)
    {
        _entityManager = entityManager;
    }

    /// <summary>
    /// 设置当前相机
    /// </summary>
    /// <param name="camera">相机对象</param>
    public void BindCurrentMouseInteractionService(MouseInteractionService mouseInteractionService)
    {
        if (_mouseInteractionService is null)
        {
            _mouseInteractionService = mouseInteractionService;
            mouseInteractionService.ViewportChangedEvent += OnViewportChanged;
        }
    }

    private void OnViewportChanged(object? sender, ViewportChangedEventArgs e)
    {
        UpdateViewportInfo();
    }

    /// <summary>
    /// 获取当前视口信息
    /// </summary>
    /// <returns>视口信息对象</returns>
    public void UpdateViewportInfo()
    {
        if(_currentCamera is null)
        {
            return;
        }

        // 获取屏幕视口
        var screenViewport = _currentCamera.ScreenViewport;

        // 计算世界坐标视口
        float worldWidth = (_currentCamera.Position.Z / _currentCamera.Projection.Scale.X * 2);
        float worldHeight = (_currentCamera.Position.Z / _currentCamera.Projection.Scale.Y * 2);

        var worldViewport = new RectangleF(
            _currentCamera.Position.X - worldWidth / 2,
            _currentCamera.Position.Y - worldHeight / 2,
            worldWidth,
            worldHeight
        );

        Viewport.ScreenViewport = new((int)screenViewport.X, (int)screenViewport.Y, (int)screenViewport.Width, (int)screenViewport.Height);
        Viewport.WorldViewport = worldViewport;
        Viewport.ScrollScale = _currentCamera.Position.Z;
    }

    /// <summary>
    /// 屏幕坐标转换为世界坐标
    /// </summary>
    /// <param name="screenX">屏幕X坐标</param>
    /// <param name="screenY">屏幕Y坐标</param>
    /// <returns>世界坐标</returns>
    public Vector2 ScreenToWorld(float screenX, float screenY)
    {
        if (_mouseInteractionService == null)
        {
            return Vector2.Zero;
        }

        return _mouseInteractionService.Screen2World(new((int)screenX, (int)screenY));
    }

    /// <summary>
    /// 世界坐标转换为屏幕坐标
    /// </summary>
    /// <param name="worldX">世界X坐标</param>
    /// <param name="worldY">世界Y坐标</param>
    /// <returns>屏幕坐标</returns>
    public Vector2 WorldToScreen(float worldX, float worldY)
    {
        if (_mouseInteractionService == null)
        {
            return Vector2.Zero;
        }

        return _mouseInteractionService.WorldToScreen(new((int)worldX, (int)worldY));
    }

    /// <summary>
    /// 经纬度转换为世界坐标
    /// </summary>
    /// <param name="longitude">经度</param>
    /// <param name="latitude">纬度</param>
    /// <returns>世界坐标</returns>
    public Vector2 GeoToWorld(double longitude, double latitude)
    {
        if (_earthService == null)
        {
            return Vector2.Zero;
        }

        // 使用地球服务进行投影转换
        var projection = _earthService.CurrentProjection;
        if (projection != null)
        {
            projection.Geo2Index(longitude, latitude, out var worldX, out var worldY);
            return new Vector2((float)worldX, (float)worldY);
        }

        return Vector2.Zero;
    }

    /// <summary>
    /// 世界坐标转换为经纬度
    /// </summary>
    /// <param name="worldX">世界X坐标</param>
    /// <param name="worldY">世界Y坐标</param>
    /// <returns>经纬度（X为经度，Y为纬度）</returns>
    public Vector2 WorldToGeo(float worldX, float worldY)
    {
        if (_earthService == null)
        {
            return Vector2.Zero;
        }

        // 使用地球服务进行投影转换
        var projection = _earthService.CurrentProjection;
        if (projection != null)
        {
            projection.Index2Geo(worldX, worldY, out var lon, out var lat);
            return new Vector2((float)lon, (float)lat);
        }

        return Vector2.Zero;
    }

    /// <summary>
    /// 屏幕坐标转换为经纬度
    /// </summary>
    /// <param name="screenX">屏幕X坐标</param>
    /// <param name="screenY">屏幕Y坐标</param>
    /// <returns>经纬度（X为经度，Y为纬度）</returns>
    public Vector2 ScreenToGeo(float screenX, float screenY)
    {
        // 先转换为世界坐标，再转换为经纬度
        var worldPos = ScreenToWorld(screenX, screenY);
        return WorldToGeo(worldPos.X, worldPos.Y);
    }

    /// <summary>
    /// 经纬度转换为屏幕坐标
    /// </summary>
    /// <param name="longitude">经度</param>
    /// <param name="latitude">纬度</param>
    /// <returns>屏幕坐标</returns>
    public Vector2 GeoToScreen(double longitude, double latitude)
    {
        // 先转换为世界坐标，再转换为屏幕坐标
        var worldPos = GeoToWorld(longitude, latitude);
        return WorldToScreen(worldPos.X, worldPos.Y);
    }
}