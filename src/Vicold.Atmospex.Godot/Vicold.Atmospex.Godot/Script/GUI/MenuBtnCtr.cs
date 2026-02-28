using Godot;
using System;

public partial class MenuBtnCtr : Control
{
    public override void _Ready()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var interaction = RenderModuleService.GetService<IInteractionService>();
    }

    private void OnMenuButtonPressed()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var interaction = RenderModuleService.GetService<IInteractionService>();
        // interaction.Order.Execute("OpenMenu", null);
    }
}