using Godot;
using System;
using Vicold.Atmospex.Godot.Model;
using Vicold.Atmospex.Layer;

internal partial class LayerManagerItem : MarginContainer, ILayerManagerItem
{
    private ILayerManager _layerManager;

    private TextureButton _deleteBtn;
    private TextureButton _visibleBtn;

    public override void _Ready()
    {
        _deleteBtn = GetNode<TextureButton>("ContentContainer/LayerDeleteBtn");
        _visibleBtn = GetNode<TextureButton>("ContentContainer/LayerHideBtn");
    }

    public string ID { get; private set; }

    public string LayerName { get; set; }

    public bool IsSystemLayer
    {
        get
        {
            return !_deleteBtn.Visible;
        }
        set
        {
            _deleteBtn.Visible = !value;
        }
    }

    public void SetItemVisible(bool isShow)
    {
        _visibleBtn.SetPressed(!isShow);
    }

    public void SetMessage(string id, string name)
    {
        ID = id;
        LayerName = name;
        if (HasNode("ContentContainer/LayerName"))
        {
            var label = GetNode<Label>("ContentContainer/LayerName");
            label.Text = LayerName;
            label.TooltipText = LayerName;
        }
    }

    #region signal

    public void _on_LayerHideBtn_toggled(bool isShow)
    {
        var layerManager = Vicold.Atmospex.Godot.App.GetService<ILayerModuleService>().LayerManager;
        if (layerManager.TryGetLayer(ID, out var layer))
        {
            layer.IsVisible = !isShow;
        }
    }

    public void _on_LayerDeleteBtn_button_up()
    {
        var layerManager = Vicold.Atmospex.Godot.App.GetService<ILayerModuleService>().LayerManager;
        if (layerManager.TryGetLayer(ID, out var layer))
        {
            layerManager.RemoveLayer(layer);
        }
    }

    #endregion
}