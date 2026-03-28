using Godot;
using System;
using System.Linq;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Layer.Events;
using Vicold.Atmospex.Godot.Model;

public partial class LayerManagerPad : VBoxContainer
{
    private VBoxContainer layerList;
    private ILayerManager _layerManager;

    public override void _Ready()
    {
        layerList = GetNode<VBoxContainer>("ScrollContainer/LayerList");

        _layerManager = Vicold.Atmospex.Godot.App.GetService<ILayerModuleService>().LayerManager;
        _layerManager.OnLayerAdded += OnLayerAdded;
        _layerManager.OnLayerUpdated += OnLayerUpdated;
        _layerManager.OnLayerRemoved += OnLayerRemoved;
        _layerManager.OnLayerVisibleChanged += OnLayerVisibleChanged;
    }

    private void OnLayerAdded(object sender, LayerChangedEventArgs e)
    {
        // 保存事件参数，以便在主线程中使用
        var layerId = e.LayerId;
        var layerName = e.LayerName;
        var isSystemLayer = e.IsSystemLayer;
        var canFlip = e.CanFlip;

        // 使用call_deferred在主线程中添加子节点
        CallDeferred("AddLayerItem", layerId, layerName, isSystemLayer, canFlip);
    }

    private void AddLayerItem(string layerId, string layerName, bool isSystemLayer, bool canFlip)
    {
        ILayerManagerItem layerItem;

        if (canFlip)
        {
            var item = (PackedScene)GD.Load("res://Scene/LayerManagerFlipItem.tscn");
            var node = item.Instantiate<LayerManagerFlipItem>();
            layerList.AddChild(node);
            layerItem = node;
        }
        else
        {
            var item = (PackedScene)GD.Load("res://Scene/LayerManagerItem.tscn");
            var node = item.Instantiate<LayerManagerItem>();
            layerList.AddChild(node);
            layerItem = node;
        }
        layerItem.SetMessage(layerId, layerName);
        layerItem.IsSystemLayer = isSystemLayer;
    }

    private void OnLayerUpdated(object sender, LayerChangedEventArgs e)
    {
        var children = layerList.GetChildren();
        foreach (var item in children)
        {
            if (item is ILayerManagerItem layerItem && layerItem.ID == e.LayerId)
            {
                var provider = e.Layer.DataProvider;
                if (item is LayerManagerFlipItem flipItem)
                {
                    flipItem.SetMessage(e.LayerId, e.LayerName);
                }
                else if (item is LayerManagerItem unflipItem)
                {
                    unflipItem.SetMessage(e.LayerId, e.LayerName);
                }

                return;
            }
        }
    }

    private void OnLayerRemoved(object sender, LayerChangedEventArgs e)
    {
        var items = layerList.GetChildren();
        var layerCount = items.Count;
        for (var i = layerCount - 1; i >= 0; i--)
        {
            var item = items[i] as ILayerManagerItem;
            var node = items[i] as Node;
            if (item != null && item.ID == e.LayerId)
            {
                layerList.RemoveChild(node);
                break;
            }
        }
    }

    private void OnLayerVisibleChanged(object sender, LayerChangedEventArgs e)
    {
        // 暂时留空，根据需要实现
    }

    public void _on_HideLayerBtn_toggled(bool isShow)
    {
        var items = layerList.GetChildren();
        var layerCount = items.Count;
        for (var i = layerCount - 1; i >= 0; i--)
        {
            var node = items[i] as ILayerManagerItem;
            if (node != null && !node.IsSystemLayer)
            {
                node.SetItemVisible(!isShow);
            }
        }
    }

    public void _on_CloseLayerBtn_button_up()
    {
        var layers = _layerManager.GetAllLayers();
        var layerArray = layers.ToArray();
        for (var i = layerArray.Length - 1; i >= 0; i--)
        {
            var layer = layerArray[i];
            if (!layer.IsSystemLayer)
            {
                _layerManager.RemoveLayer(layer);
            }
        }
    }
}