using Godot;
using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Godot.Frame;

namespace Vicold.Atmospex.Godot;

public partial class Root : Node
{
    private IAppService _appService;
    private IRenderModuleService _renderModuleService;
    
    public override void _Ready()
    {
        // 初始化应用程序
        InitializeApp();
        
        // 初始化渲染服务
        InitializeRenderService();
        
        // 初始化相机
        InitializeCamera();
    }
    
    private void InitializeApp()
    {
        // 这里应该初始化应用程序服务，参考Vicold.Atmospex.Shell的实现
        // 由于是示例，暂时只做基础初始化
        GD.Print("App initialized");
    }
    
    private void InitializeRenderService()
    {
        // 这里应该初始化渲染服务，获取依赖注入的服务
        // 由于是示例，暂时只做基础初始化
        GD.Print("Render service initialized");
    }
    
    private void InitializeCamera()
    {
        // 获取相机节点
        var camera = GetNode<Camera2D>("MapBox/Camera2D");
        
        if (camera != null && _renderModuleService is RenderModuleService renderService)
        {
            renderService.SetCurrentCamera(camera);
        }
        
        GD.Print("Camera initialized");
    }
    
    public override void _Process(double delta)
    {
        // 处理帧更新
    }
}