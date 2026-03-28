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
    private Camera2D? _currentCamera2D;
    private Camera3D? _currentCamera3D;
    
    public static RenderModuleService? Current { get; private set; }

    public RenderModuleService(IAppService appService, IEarthModuleService earthService, ILayerModuleService layerModuleService)
    {
        _appService = appService;
        Current = this;

        _earthService = earthService;
        _layerModuleService = layerModuleService;

        // 监听图层事件
        //layerModuleService.LayerManager.OnLayerAdded += (s, e) =>
        //{
        //    if (e.Layer is IRenderLayer el)
        //    {
        //        el.Draw();
        //    }
        //};

        //layerModuleService.LayerManager.OnLayerUpdating += (s, e) =>
        //{
        //    if (e.Layer is IRenderLayer el)
        //    {
        //        el.Erase();
        //    }
        //};

        //layerModuleService.LayerManager.OnLayerUpdated += (s, e) =>
        //{
        //    if (e.Layer is IRenderLayer el)
        //    {
        //        el.Draw();
        //    }
        //};

        //layerModuleService.LayerManager.OnLayerRemoved += (s, e) =>
        //{
        //    if (e.Layer is IRenderLayer el)
        //    {
        //        el.Erase();
        //    }
        //};
    }

    public ViewportInfo Viewport { get; } = new();

    public VisionGate Vision { get; }

    // 设置当前相机（2D）
    public void SetCurrentCamera(Camera2D camera)
    {
        _currentCamera2D = camera;
        _currentCamera3D = null;
        UpdateViewportInfo();
    }

    // 设置当前相机（3D）
    public void SetCurrentCamera(Camera3D camera)
    {
        _currentCamera3D = camera;
        _currentCamera2D = null;
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
        if (_currentCamera2D is not null)
        {
            // 获取屏幕尺寸
            var viewport = _currentCamera2D.GetViewport();
            var screenSize = viewport.GetVisibleRect().Size;
            
            // 计算世界坐标视口
            float zoom = _currentCamera2D.Zoom.X;
            float worldWidth = screenSize.X / zoom;
            float worldHeight = screenSize.Y / zoom;

            var worldViewport = new Rect2(
                _currentCamera2D.Position - new Vector2(worldWidth / 2, worldHeight / 2),
                new Vector2(worldWidth, worldHeight)
            );

            Viewport.ScreenViewport = new Rect2I(Vector2I.Zero, new Vector2I((int)screenSize.X, (int)screenSize.Y));
            Viewport.WorldViewport = worldViewport;
            Viewport.ScrollScale = zoom;
        }
        else if (_currentCamera3D is not null)
        {
            // 获取屏幕尺寸
            var viewport = _currentCamera3D.GetViewport();
            var screenSize = viewport.GetVisibleRect().Size;
            
            // 对于3D相机，使用默认视口信息
            Viewport.ScreenViewport = new Rect2I(Vector2I.Zero, new Vector2I((int)screenSize.X, (int)screenSize.Y));
            Viewport.WorldViewport = new Rect2(Vector2.Zero, screenSize);
            Viewport.ScrollScale = 1.0f;
        }
    }

    public Vector2 ScreenToWorld(float screenX, float screenY)
    {
        if (_currentCamera2D is not null)
        {
            var viewport = _currentCamera2D.GetViewport();
            var screenSize = viewport.GetVisibleRect().Size;
            
            // 计算屏幕坐标相对于视口中心的偏移量
            Vector2 screenOffset = new Vector2(screenX, screenY) - screenSize / 2;
            
            // 根据相机缩放调整偏移量
            Vector2 worldOffset = screenOffset / _currentCamera2D.Zoom;
            
            // 计算世界坐标
            Vector2 worldPos = _currentCamera2D.Position + worldOffset;
            
            return worldPos;
        }
        else if (_currentCamera3D is not null)
        {
            // 使用射线检测获取世界坐标（假设地面在Z=0平面）
            var cameraRay = _currentCamera3D.ProjectRayNormal(new Vector2(screenX, screenY));
            if (cameraRay.Z != 0)
            {
                var t = -_currentCamera3D.Position.Z / cameraRay.Z;
                var worldPos = _currentCamera3D.Position + cameraRay * t;
                return new Vector2(worldPos.X, worldPos.Y);
            }
        }

        return Vector2.Zero;
    }

    public Vector2 WorldToScreen(float worldX, float worldY)
    {
        if (_currentCamera2D is not null)
        {
            var viewport = _currentCamera2D.GetViewport();
            var screenSize = viewport.GetVisibleRect().Size;
            
            // 计算世界坐标相对于相机位置的偏移量
            Vector2 worldOffset = new Vector2(worldX, worldY) - _currentCamera2D.Position;
            
            // 根据相机缩放调整偏移量
            Vector2 screenOffset = worldOffset * _currentCamera2D.Zoom;
            
            // 计算屏幕坐标
            Vector2 screenPos = screenSize / 2 + screenOffset;
            
            return screenPos;
        }
        else if (_currentCamera3D is not null)
        {
            // 对于3D相机，使用视口投影
            var viewport = _currentCamera3D.GetViewport();
            var screenSize = viewport.GetVisibleRect().Size;
            var worldPos = new Vector3(worldX, worldY, 0);
            var screenPos = _currentCamera3D.UnprojectPosition(worldPos);
            return screenPos;
        }

        return Vector2.Zero;
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
        if (_currentCamera2D is { })
        {
            _currentCamera2D.Position = world;
            UpdateViewportInfo();
        }
        else if (_currentCamera3D is { })
        {
            _currentCamera3D.Position = new Vector3(world.X, world.Y, _currentCamera3D.Position.Z);
            UpdateViewportInfo();
        }
    }

    public void SetLaunchGeoPosition(float lon, float lat, float z)
    {
        var world = GeoToWorld(lon, lat);
        if (_currentCamera2D is { })
        {
            _currentCamera2D.Position = world;
            _currentCamera2D.Zoom = new Vector2(1f / z, 1f / z);
            UpdateViewportInfo();
        }
        else if (_currentCamera3D is { })
        {
            _currentCamera3D.Position = new Vector3(world.X, world.Y, z);
            UpdateViewportInfo();
        }
    }

    public (float lon, float lat) GetCurrentGeoPosition()
    {
        if (_currentCamera2D is { })
        {
            var geo = WorldToGeo(_currentCamera2D.Position.X, _currentCamera2D.Position.Y);
            return (geo.X, geo.Y);
        }
        else if (_currentCamera3D is { })
        {
            var geo = WorldToGeo(_currentCamera3D.Position.X, _currentCamera3D.Position.Y);
            return (geo.X, geo.Y);
        }

        return (0, 0);
    }

    public void ResetCamera()
    {
        if (_currentCamera2D is { })
        {
            _currentCamera2D.Position = Vector2.Zero;
            _currentCamera2D.Zoom = Vector2.One;
            UpdateViewportInfo();
        }
        else if (_currentCamera3D is { })
        {
            _currentCamera3D.Position = new Vector3(0, 0, 10);
            _currentCamera3D.LookAt(Vector3.Zero, Vector3.Up);
            UpdateViewportInfo();
        }
    }

    public static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }
}