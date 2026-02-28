using Godot;
using Vicold.Atmospex.Godot.Frame;

namespace Vicold.Atmospex.Godot;

public partial class MapBox : Node2D
{
    private Camera2D _camera;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;
    
    public override void _Ready()
    {
        _camera = GetNode<Camera2D>("Camera2D");
        
        // 设置相机初始属性
        _camera.Position = Vector2.Zero;
        _camera.Zoom = Vector2.One;
        
        GD.Print("MapBox initialized");
    }
    
    public override void _Input(InputEvent @event)
    {
        // 处理鼠标事件
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            HandleMouseButtonEvent(mouseButtonEvent);
        }
        
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            HandleMouseMotionEvent(mouseMotionEvent);
        }
    }
    
    private void HandleMouseButtonEvent(InputEventMouseButton @event)
    {
        if (@event.ButtonIndex == MouseButton.Left)
        {
            if (@event.Pressed)
            {
                _isDragging = true;
                _lastMousePos = @event.Position;
            }
            else
            {
                _isDragging = false;
            }
        }
        // 处理鼠标滚轮事件
        else if (@event.ButtonIndex == MouseButton.WheelUp)
        {
            HandleZoom(1.1f, @event.Position);
        }
        else if (@event.ButtonIndex == MouseButton.WheelDown)
        {
            HandleZoom(0.9f, @event.Position);
        }
    }
    
    private void HandleMouseMotionEvent(InputEventMouseMotion @event)
    {
        if (_isDragging && _camera != null)
        {
            // 计算鼠标移动距离
            Vector2 delta = _lastMousePos - @event.Position;
            
            // 转换为世界坐标移动距离
            delta /= _camera.Zoom.X;
            
            // 更新相机位置
            _camera.Position += delta;
            
            // 更新视口信息
            UpdateViewport();
            
            _lastMousePos = @event.Position;
        }
    }
    
    private void HandleZoom(float zoomFactor, Vector2 mousePos)
    {
        if (_camera != null)
        {
            // 限制缩放范围
            Vector2 newZoom = _camera.Zoom * zoomFactor;
            newZoom = new Vector2(
                Mathf.Clamp(newZoom.X, 0.1f, 10.0f),
                Mathf.Clamp(newZoom.Y, 0.1f, 10.0f)
            );
            
            // 计算鼠标在世界坐标中的位置
            Vector2 worldPos = ScreenToWorld(mousePos.X, mousePos.Y);
            
            // 更新相机缩放
            _camera.Zoom = newZoom;
            
            // 调整相机位置，使鼠标位置保持不变
            Vector2 newWorldPos = ScreenToWorld(mousePos.X, mousePos.Y);
            _camera.Position += worldPos - newWorldPos;
            
            // 更新视口信息
            UpdateViewport();
        }
    }
    
    private Vector2 ScreenToWorld(float screenX, float screenY)
    {
        if (_camera is null)
        {
            return Vector2.Zero;
        }

        var viewport = _camera.GetViewport();
        var screenSize = viewport.GetVisibleRect().Size;
        
        // 计算屏幕坐标相对于视口中心的偏移量
        Vector2 screenOffset = new Vector2(screenX, screenY) - screenSize / 2;
        
        // 根据相机缩放调整偏移量
        Vector2 worldOffset = screenOffset / _camera.Zoom;
        
        // 计算世界坐标
        Vector2 worldPos = _camera.Position + worldOffset;
        
        return worldPos;
    }
    
    private void UpdateViewport()
    {
        // 更新渲染服务的视口信息
        if (RenderModuleService.Current != null)
        {
            RenderModuleService.Current.UpdateViewportInfo();
        }
    }
}