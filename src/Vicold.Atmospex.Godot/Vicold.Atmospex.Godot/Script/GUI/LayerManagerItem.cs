using Godot;
using System;

public partial class LayerManagerItem : Control
{
    public override void _Ready()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var toggleButton = GetNode<CheckBox>("ToggleButton");
        // if (toggleButton != null)
        // {
        //     toggleButton.Toggled += OnToggleButtonToggled;
        // }
    }

    private void OnToggleButtonToggled(bool isToggled)
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var layerId = Name;
        // var layerManager = RenderModuleService.GetService<ILayerManager>();
        // var layer = layerManager.GetLayer(layerId);
        // if (layer != null)
        // {
        //     layer.Visible = isToggled;
        // }
    }
}