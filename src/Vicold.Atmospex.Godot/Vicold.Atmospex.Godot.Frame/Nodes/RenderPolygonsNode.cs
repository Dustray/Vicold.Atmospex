using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Godot.Frame.Layers;

namespace Vicold.Atmospex.Godot.Frame.Nodes;

public partial class RenderPolygonsNode : Node2D, ILayerNode, IRenderNode
{
    private float _scale = 1;
    private VectorLine[] _polygons;
    private bool _disposed;

    public string ID { get; set; }
    public bool Visible
    {
        get => IsVisible;
        set => IsVisible = value;
    }
    public bool IsTileEnabled { get; set; }
    private bool IsVisible { get; set; } = true;

    public void SetPolygons(VectorLine[] data, float scale)
    {
        _polygons = data;
        _scale = scale;
        Task.Run(() => ThreadAdd());
    }

    public void ResetPolygons(VectorLine[] data)
    {
        _polygons = data;
        var children = GetChildren();
        for (var i = children.Count - 1; i >= 0; i--)
        {
            var node = (Node)children[i];
            RemoveChild(node);
        }

        Task.Run(() => ThreadAdd());
    }

    public override void _Ready()
    {
        base._Ready();
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
        if (_polygons == null)
            return;

        foreach (var polygon in _polygons)
        {
            var godotPoints = new global::Godot.Vector2[polygon.Data.Length];
            for (int i = 0; i < polygon.Data.Length; i++)
            {
                godotPoints[i] = new global::Godot.Vector2(polygon.Data[i].X, polygon.Data[i].Y);
            }

            if ((polygon.PolygonType & PolygonType.Fill) != PolygonType.None)
            {
                var polygon2D = new Polygon2D();
                polygon2D.Polygon = godotPoints;
                polygon2D.Color = ToGodotColor(polygon.FillColor);
                polygon2D.ZIndex = ZIndex;
                CallDeferred("add_child", polygon2D);
            }

            if ((polygon.PolygonType & PolygonType.Line) != PolygonType.None)
            {
                var line2D = new Line2D();
                line2D.Points = godotPoints;
                line2D.DefaultColor = ToGodotColor(polygon.LineColor);
                line2D.Width = polygon.Width * _scale;
                line2D.ZIndex = ZIndex;
                CallDeferred("add_child", line2D);
            }
        }
    }

    private Color ToGodotColor(System.Drawing.Color color)
    {
        return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

    public new void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _polygons = null;
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
}
