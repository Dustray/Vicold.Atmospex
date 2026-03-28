using Godot;
using Newtonsoft.Json;
using System;
using Vicold.Atmospex.Configration;
using Vicold.Atmospex.Godot;
using Vicold.Atmospex.Godot.Frame.Layers;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Layer.Tool;
using Windows.System;

public partial class MapBox : Node2D
{
	private Camera2D _camera;
    private ILayerManager _layerManager;



    public MapBox()
    {
        App.Vision.OnNodeLoad = OnNodeLoad;
        App.Vision.OnNodeRemove = OnNodeRemove;
        App.Vision.OnNodeVisibleChanged = OnNodeVisibleChanged;
    }

    public override void _Ready()
    {
        App.GetService<Vicold.Atmospex.Core.ICoreModuleService>().OnViewStart.Invoke();
    }

    public void OnNodeLoad(ILayerNode node)
    {
        if (node is Node2D node2d)
        {
            AddChild(node2d);
        }
    }

    public void OnNodeRemove(ILayerNode node)
    {
        if (node is Node2D node2d)
        {
            RemoveChild(node2d);
            node2d.QueueFree();
        }
    }

    public void OnNodeVisibleChanged(ILayerNode node, bool isVisible)
    {
        if (node is Node2D node2d)
        {
            node2d.Visible = isVisible;
        }
    }
}
