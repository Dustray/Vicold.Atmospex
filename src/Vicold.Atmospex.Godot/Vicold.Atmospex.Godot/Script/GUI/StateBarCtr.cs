using Godot;
using System;

public partial class StateBarCtr : Control
{
    public override void _Ready()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var interaction = RenderModuleService.GetService<IInteractionService>();
        // interaction.OnMouseMove += OnMouseMove;
    }

    public override void _Input(InputEvent @event)
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // base._Input(@event);
    }

    private void OnMouseMove(object sender, EventArgs e)
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var args = e as MouseMoveEventArgs;
        // if (args != null)
        // {
        //     // 更新状态栏显示
        // }
    }
}