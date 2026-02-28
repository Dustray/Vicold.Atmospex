using Godot;
using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Godot.Frame.Layers;

namespace Vicold.Atmospex.Godot.Frame;

public class RenderModuleService : IRenderModuleService
{
    private static IAppService? _appService;
    private readonly IEarthModuleService _earthService;
    private readonly ILayerModuleService _layerModuleService;

    // 相机引用
    private Camera2D? _currentCamera;
    
    public static RenderModuleService? Current { get; private set; }

    public RenderModuleService(IAppService appService, IEarthModuleService earthService, ILayerModuleService layerModuleService)
    {
        _appService = appService;
        Current = this;

        _earthService = earthService;
        _layerModuleService = layerModuleService;

        // 监听图层事件
        layerModuleService.LayerManager.OnLayerAdded += (s, e) =>
        {
            if (e.Layer is IRenderLayer el)
            {
                el.Draw();
            }
        };

        layerModuleService.LayerManager.OnLayerUpdating += (s, e) =>
        {
            if (e.Layer is IRenderLayer el)
            {
                el.Erase();
            }
        };

        layerModuleService.LayerManager.OnLayerUpdated += (s, e) =>
        {
            if (e.Layer is IRenderLayer el)
            {
                el.Draw();
            }
        };

        layerModuleService.LayerManager.OnLayerRemoved += (s, e) =>
        {
            if (e.Layer is IRenderLayer el)
            {
                el.Erase();
            }
        };
    }

    public ViewportInfo Viewport { get; } = new();

    // 设置当前相机
    public void SetCurrentCamera(Camera2D camera)
    {
        _currentCamera = camera;
        UpdateViewportInfo();
    }

    // 初始化
    public void Initialize()
    {
        // 可以在这里进行初始化操作
    }

    // 更新视口信息
    public void UpdateViewportInfo()
    {
        if (_currentCamera is null)
        {
            return;
        }

        // 获取屏幕尺寸
        var viewport = _currentCamera.GetViewport();
        var screenSize = viewport.GetVisibleRect().Size;
        
        // 计算世界坐标视口
        float zoom = _currentCamera.Zoom.X;
        float worldWidth = screenSize.X / zoom;
        float worldHeight = screenSize.Y / zoom;

        var worldViewport = new Rect2(
            _currentCamera.Position - new Vector2(worldWidth / 2, worldHeight / 2),
            new Vector2(worldWidth, worldHeight)
        );

        Viewport.ScreenViewport = new Rect2I(Vector2I.Zero, new Vector2I((int)screenSize.X, (int)screenSize.Y));
        Viewport.WorldViewport = worldViewport;
        Viewport.ScrollScale = zoom;
    }

    public Vector2 ScreenToWorld(float screenX, float screenY)
    {
        if (_currentCamera is null)
        {
            return Vector2.Zero;
        }

        var viewport = _currentCamera.GetViewport();
        var screenSize = viewport.GetVisibleRect().Size;
        
        // 计算屏幕坐标相对于视口中心的偏移量
        Vector2 screenOffset = new Vector2(screenX, screenY) - screenSize / 2;
        
        // 根据相机缩放调整偏移量
        Vector2 worldOffset = screenOffset / _currentCamera.Zoom;
        
        // 计算世界坐标
        Vector2 worldPos = _currentCamera.Position + worldOffset;
        
        return worldPos;
    }

    public Vector2 WorldToScreen(float worldX, float worldY)
    {
        if (_currentCamera is null)
        {
            return Vector2.Zero;
        }

        var viewport = _currentCamera.GetViewport();
        var screenSize = viewport.GetVisibleRect().Size;
        
        // 计算世界坐标相对于相机位置的偏移量
        Vector2 worldOffset = new Vector2(worldX, worldY) - _currentCamera.Position;
        
        // 根据相机缩放调整偏移量
        Vector2 screenOffset = worldOffset * _currentCamera.Zoom;
        
        // 计算屏幕坐标
        Vector2 screenPos = screenSize / 2 + screenOffset;
        
        return screenPos;
    }

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

    public Vector2 ScreenToGeo(float screenX, float screenY)
    {
        // 先转换为世界坐标，再转换为经纬度
        var worldPos = ScreenToWorld(screenX, screenY);
        return WorldToGeo(worldPos.X, worldPos.Y);
    }

    public Vector2 GeoToScreen(double longitude, double latitude)
    {
        // 先转换为世界坐标，再转换为屏幕坐标
        var worldPos = GeoToWorld(longitude, latitude);
        return WorldToScreen(worldPos.X, worldPos.Y);
    }

    public void SetLaunchGeoPosition(float lon, float lat)
    {
        var world = GeoToWorld(lon, lat);
        if (_currentCamera is { })
        {
            _currentCamera.Position = world;
            UpdateViewportInfo();
        }
    }

    public void SetLaunchGeoPosition(float lon, float lat, float z)
    {
        var world = GeoToWorld(lon, lat);
        if (_currentCamera is { })
        {
            _currentCamera.Position = world;
            _currentCamera.Zoom = new Vector2(1f / z, 1f / z);
            UpdateViewportInfo();
        }
    }

    public (float lon, float lat) GetCurrentGeoPosition()
    {
        if (_currentCamera is { })
        {
            var geo = WorldToGeo(_currentCamera.Position.X, _currentCamera.Position.Y);
            return (geo.X, geo.Y);
        }

        return (0, 0);
    }

    public void ResetCamera()
    {
        if (_currentCamera is { })
        {
            _currentCamera.Position = Vector2.Zero;
            _currentCamera.Zoom = Vector2.One;
            UpdateViewportInfo();
        }
    }

    public static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }
}