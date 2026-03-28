using Godot;
using System;
using System.Threading.Tasks;
using Vicold.Atmospex.Godot.Frame;
using Vicold.Atmospex.Godot.Frame.Services;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Earth.Projection;

public partial class StateBarCtr : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private IInteractionService _interactionService;
    private IEarthModuleService _earthService;
    private Label _fpsLabel;
    private Label _positionLabel;

    public override void _Ready()
    {
        _interactionService = RenderModuleService.GetService<IInteractionService>();
        _earthService = Vicold.Atmospex.Godot.App.GetService<IEarthModuleService>();
        _fpsLabel = GetNode<Label>("MarginCtr/StateFrame/RightCtr/FPSValue");
        _positionLabel = GetNode<Label>("MarginCtr/StateFrame/LeftCtr/PositionValue");
        // TODO: 连接鼠标移动事件
    }

    public override void _Process(double delta)
    {
        _fpsLabel.Text = $"{Engine.GetFramesPerSecond()}";
    }

    private void OnMouseMove(object sender, EventArgs e)
    {
        // TODO: 实现鼠标移动事件处理
    }
}