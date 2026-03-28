using Godot;
using System;
using Vicold.Atmospex.Godot.Model;

internal partial class LayerManagerFlipItem : MarginContainer, ILayerManagerItem
{
    private LayerManagerItem _layerItem;

    public LayerManagerFlipItem()
    {
    }

    public override void _Ready()
    {
        _layerItem = GetNode<LayerManagerItem>("FrameContainer/LayerItem");
    }

    public bool IsSystemLayer
    {
        get
        {
            return _layerItem.IsSystemLayer;
        }
        set
        {
            _layerItem.IsSystemLayer = value;
        }
    }

    public string ID => _layerItem.ID;

    public void SetItemVisible(bool isShow)
    {
        _layerItem.SetItemVisible(isShow);
    }

    public void SetMessage(string id, string name)
    {
        _layerItem.SetMessage(id, name);
    }

    public void _on_FlipLeftBtn_button_down()
    {
        // 暂时留空，根据需要实现
    }

    public void _on_FlipRightBtn_button_down()
    {
        // 暂时留空，根据需要实现
    }

    public void _on_Merge_button_down()
    {
        // 暂时留空，根据需要实现
    }

    internal void setElementMessage(string elemName, string elemValue)
    {
        var nameNode = GetNode<Label>("FrameContainer/InfoContainer/MsgContainer/ElemName");
        var valueNode = GetNode<Label>("FrameContainer/InfoContainer/MsgContainer/ElemValue");
        nameNode.Text = elemName;
        valueNode.Text = elemValue;
    }
}