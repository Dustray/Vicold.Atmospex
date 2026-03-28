using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.DataService.Provider;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Godot.Frame.Layers;
using Vicold.Atmospex.Style;

namespace Vicold.Atmospex.Godot.Frame.Nodes;

public partial class RenderTextureNode : Node2D, ILayerNode, IRenderNode
{
    private ImageTexture _texture;
    private Image _image;

    public string ID { get; set; }
    public bool Visible
    {
        get => IsVisible;
        set => IsVisible = value;
    }
    public bool IsTileEnabled { get; set; }
    private bool IsVisible { get; set; } = true;

    public float StartX { get; set; }
    public float StartY { get; set; }
    public float WorldWidth { get; set; }
    public float WorldHeight { get; set; }

    public void SetTexture(GridDataProvider provider, IPalette palette)
    {
        provider.LoadData();
        var data = provider.GetData();
        StartX = (float)provider.StartLongitude;
        StartY = (float)provider.StartLatitude;
        WorldWidth = (float)(provider.Width * provider.LonInterval);
        WorldHeight = (float)(provider.Height * provider.LatInterval);

        _image = MakeBitmapImage(data, provider, palette);
        _texture = ImageTexture.CreateFromImage(_image);

        if (provider.SmoothType != -1)
        {
            data.Dispose();
        }
    }

    public void ResetImage(GridDataProvider provider, IPalette palette)
    {
        _image?.Dispose();
        _texture?.Dispose();

        var data = provider.GetData();
        _image = MakeBitmapImage(data, provider, palette);
        _texture = ImageTexture.CreateFromImage(_image);

        if (provider.SmoothType != -1)
        {
            data.Dispose();
        }
        QueueRedraw();
    }

    public override void _Ready()
    {
        base._Ready();
        if (_texture == null && _image != null)
        {
            _texture = ImageTexture.CreateFromImage(_image);
        }
    }

    public override void _Draw()
    {
        base._Draw();
        if (_texture != null)
        {
            DrawTexture(_texture, new Vector2(StartX, StartY));
        }
    }

    private Image MakeBitmapImage(GridData data, GridDataProvider provider, IPalette palette)
    {
        var width = data.Width;
        var height = data.Height;
        var image = Image.Create(width, height, false, Image.Format.Rgba8);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var value = data.ReadFloat(x, y);
                var colorItem = palette.Select(value);
                var color = new Color(colorItem.R / 255f, colorItem.G / 255f, colorItem.B / 255f, colorItem.A / 255f);
                image.SetPixel(x, y, color);
            }
        }

        return image;
    }

    public new void Dispose()
    {
        _image?.Dispose();
        _texture?.Dispose();
        _image = null;
        _texture = null;
    }

    void IRenderNode.Draw(Node? parent, RenderLayerDescription? layerDescription)
    {
        RenderDraw(parent, layerDescription);
    }

    public void RenderDraw(Node? parent = null, RenderLayerDescription? layerDescription = null)
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

    public void ResetScale(float scale)
    {
    }

    public void SetLevel(int zIndex)
    {
        ZIndex = zIndex;
    }
}
