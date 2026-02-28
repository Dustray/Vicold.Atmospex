using Godot;
using System;

public partial class LayerManagerPad : Control
{
    public override void _Ready()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var layerManager = RenderModuleService.GetService<ILayerManager>();
    }

    private void OnLayerToggle(string layerId, bool isVisible)
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var layerManager = RenderModuleService.GetService<ILayerManager>();
        // var layer = layerManager.GetLayer(layerId);
        // if (layer != null)
        // {
        //     layer.Visible = isVisible;
        // }
    }
}