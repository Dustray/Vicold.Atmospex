using Godot;
using System;

public partial class QuickButtonBar : Control
{
    public override void _Ready()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var interaction = RenderModuleService.GetService<IInteractionService>();
    }

    private void OnQuickButtonPressed(string buttonId)
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var interaction = RenderModuleService.GetService<IInteractionService>();
        // interaction.Order.Execute(buttonId, null);
    }
}