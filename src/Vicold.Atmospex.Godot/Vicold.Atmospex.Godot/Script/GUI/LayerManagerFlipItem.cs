using Godot;
using System;

public partial class LayerManagerFlipItem : MarginContainer
{
    private LayerManagerItem _layerItem;

    public override void _Ready()
    {
        _layerItem = GetNode<LayerManagerItem>("FrameContainer/LayerItem");
    }

    // 暂时注释掉，需要根据Atmospex的服务架构进行调整
    // public bool IsSystemLayer
    // {
    //     get
    //     {
    //         return _layerItem.IsSystemLayer;
    //     }
    //     set
    //     {
    //         _layerItem.IsSystemLayer = value;
    //     }
    // }

    // public string ID => _layerItem.ID;

    // public void SetItemVisible(bool isShow)
    // {
    //     _layerItem.SetItemVisible(isShow);
    // }

    // public void SetMessage(string id, string name)
    // {
    //     _layerItem.SetMessage(id, name);
    // }

    public void _on_FlipLeftBtn_button_down()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        //_dataHub.LayerFlipAsync(ID, false);
    }

    public void _on_FlipRightBtn_button_down()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        //_dataHub.LayerFlipAsync(ID, true);
    }

    public void _on_Merge_button_down()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        //_dataHub.OrderExcuteAsync(ID, "smooth");
    }

    internal void setElementMessage(string elemName, string elemValue)
    {
        var nameNode = GetNode<Label>("FrameContainer/InfoContainer/MsgContainer/ElemName");
        var valueNode = GetNode<Label>("FrameContainer/InfoContainer/MsgContainer/ElemValue");
        nameNode.Text = elemName;
        valueNode.Text = elemValue;
    }
}