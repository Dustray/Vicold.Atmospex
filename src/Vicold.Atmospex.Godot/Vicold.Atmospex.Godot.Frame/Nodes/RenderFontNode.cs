using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Godot.Frame.Layers;

namespace Vicold.Atmospex.Godot.Frame.Nodes;

public partial class RenderFontNode : Node2D, IRenderNode
{
    private float _scale = 1;
    private Theme _theme;
    private IList<VectorFont> _vectorFonts;
    private bool _disposed;

    public string ID { get; set; }
    public bool IsTileEnabled { get; set; }

    public void SetFonts(IList<VectorFont> vectorFonts)
    {
        _vectorFonts = vectorFonts;
        Task.Run(() => ThreadAdd());
    }

    public override void _Ready()
    {
        base._Ready();
        _theme = new Theme();
    }

    public override void _Draw()
    {
        base._Draw();
    }

    public void ResetScale(float scale)
    {
        _scale = scale;
        QueueRedraw();
    }

    public void SetLevel(int zIndex)
    {
        ZIndex = zIndex;
    }

    private void ThreadAdd()
    {
        if (_vectorFonts == null)
            return;

        foreach (var font in _vectorFonts)
        {
            var label = new Label();
            label.Theme = _theme;
            label.Position = new Vector2((float)font.Position.X, (float)font.Position.Y);
            label.Text = font.Font;
            
            var color = new Color(font.FontColor.R / 255f, font.FontColor.G / 255f, font.FontColor.B / 255f, font.FontColor.A / 255f);
            label.Modulate = color;
            
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Scale = new Vector2(_scale, _scale);
            CallDeferred("add_child", label);
        }
    }

    public new void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _theme?.Dispose();
        _vectorFonts?.Clear();
        _theme = null;
        _vectorFonts = null;
    }

    void IRenderNode.Draw(Node? parent, RenderLayerDescription? layerDescription)
    {
        if (parent != null && GetParent() == null)
        {
            parent.AddChild(this);
        }
        if (layerDescription != null)
        {
            ZIndex = layerDescription.Order;
        }
    }

    public void Erase()
    {
        if (GetParent() != null)
        {
            GetParent().RemoveChild(this);
        }
    }
}
