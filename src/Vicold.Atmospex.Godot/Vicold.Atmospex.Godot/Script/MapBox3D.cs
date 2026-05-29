using Godot;
using Newtonsoft.Json;
using System;
using Vicold.Atmospex.Configration;
using Vicold.Atmospex.Godot;
using Vicold.Atmospex.Godot.Frame.Layers;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Layer.Tool;
using Windows.System;

public partial class MapBox3D : Node3D
{
	private Camera3D _camera;
	private ILayerManager _layerManager;

	public MapBox3D()
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
		if (node is Node3D node3d)
		{
			CallDeferred("add_child", node3d);
            //AddChild(node3d);
        }
    }

	public void OnNodeRemove(ILayerNode node)
	{
		if (node is Node3D node3d)
		{
			CallDeferred("remove_child", node3d);
            //RemoveChild(node3d);
            node3d.QueueFree();
		}
	}

	public void OnNodeVisibleChanged(ILayerNode node, bool isVisible)
	{
		if (node is Node3D node3d)
		{
            node3d.Visible = isVisible;
		}
	}
}
