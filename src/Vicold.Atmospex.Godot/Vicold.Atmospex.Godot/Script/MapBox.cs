using Godot;
using System;
using Vicold.Atmospex.Layer;

public partial class MapBox : Node2D
{
    private Camera2D _camera;

    public override void _Ready()
    {
        _camera = GetNode<Camera2D>("MainCamera2D");
    }

    public void AddLayerNode(ILayerNode layerNode)
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var node = layerNode as Node;
        // if (node != null)
        // {
        //     AddChild(node);
        // }
    }

    public void RemoveLayerNode(ILayerNode layerNode)
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // var node = layerNode as Node;
        // if (node != null && node.IsInsideTree())
        // {
        //     node.QueueFree();
        // }
    }

    public void SetCameraValidPadding(float top, float bottom, float left, float right)
    {
        if (_camera is MainCamera2D mainCamera)
        {
            mainCamera.SetValidPadding(top, bottom, left, right);
        }
    }
}