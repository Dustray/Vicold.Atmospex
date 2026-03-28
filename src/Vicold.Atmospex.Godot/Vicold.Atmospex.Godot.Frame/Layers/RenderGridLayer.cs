using Godot;
using System.Collections.Generic;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Style;
using Vicold.Atmospex.Godot.Frame.Nodes;

namespace Vicold.Atmospex.Godot.Frame.Layers;

public class RenderGridLayer : GridLayer, IRenderLayer
{
    public RenderGridLayer(IGridDataProvider provider)
        : base(provider)
    {
        // 初始化层描述
        LayerDescription = new RenderLayerDescription
        {
            Name = "GridLayer",
            Order = 100, // 网格层通常在中间层
            IsVisible = true
        };
    }

    public RenderLayerDescription LayerDescription { get; private set; }

    protected override ILayerNode? CreateTextureNode(string ID, GridDataProvider provider, IPalette palette, IProjection prj)
    {
        var texture = new RenderTextureNode();
        texture.ID = ID;
        texture.SetTexture(provider, palette);
        return texture;
    }

    protected override void ReCreateTextureNode(TextureNode texture, GridDataProvider provider, IPalette palette, IProjection prj)
    {
        // 由于RenderTextureNode不继承自TextureNode，这里无法直接转换
        // 但我们可以通过其他方式实现，比如重新创建节点
        _layerNode = CreateTextureNode(ID, provider, palette, prj);
    }

    protected override ILayerNode? CreateLinesNode(string ID, IVectorDataProvider provider, IProjection prj)
    {
        var vectorData = provider.GetData();
        if (vectorData is LineData lineData)
        {
            var lines = Vicold.Atmospex.Earth.Tool.LineConverterTool.ToVectorLines(lineData, prj);
            var linesNode = new RenderLinesNode();
            linesNode.ID = ID;
            linesNode.SetLines(lines, 1f);
            return linesNode;
        }

        return null;
    }

    protected override void ReCreateLinesNode(LinesNode texture, IVectorDataProvider provider, IProjection prj)
    {
        // 由于RenderLinesNode不继承自LinesNode，这里无法直接转换
        // 但我们可以通过其他方式实现，比如重新创建节点
        _layerNode = CreateLinesNode(ID, provider, prj);
    }

    protected override ILayerNode? CreateLinesNode(string ID, LineData lineData, IProjection prj)
    {
        var lines = Vicold.Atmospex.Earth.Tool.LineConverterTool.ToVectorLines(lineData, prj);
        var linesNode = new RenderLinesNode();
        linesNode.ID = ID;
        linesNode.SetLines(lines, 1f);
        return linesNode;
    }

    protected override void ReCreateLinesNode(LinesNode texture, LineData lineData, IProjection prj)
    {
        // 由于RenderLinesNode不继承自LinesNode，这里无法直接转换
        // 但我们可以通过其他方式实现，比如重新创建节点
        _layerNode = CreateLinesNode(ID, lineData, prj);
    }

    protected override ILayerNode? CreatePolygonsNode(string ID, IVectorDataProvider provider, IProjection prj)
    {
        var vectorData = provider.GetData();
        if (vectorData is LineData lineData)
        {
            var polygons = Vicold.Atmospex.Earth.Tool.LineConverterTool.ToVectorLines(lineData, prj);
            var polygonsNode = new RenderPolygonsNode();
            polygonsNode.ID = ID;
            polygonsNode.SetPolygons(polygons, 1f);
            return polygonsNode;
        }

        return null;
    }

    protected override void ReCreatePolygonsNode(LinesNode texture, IVectorDataProvider provider, IProjection prj)
    {
        // 由于RenderPolygonsNode不继承自LinesNode，这里无法直接转换
        // 但我们可以通过其他方式实现，比如重新创建节点
        _layerNode = CreatePolygonsNode(ID, provider, prj);
    }

    protected override ILayerNode? CreatePolygonsNode(string ID, LineData lineData, IProjection prj)
    {
        var polygons = Vicold.Atmospex.Earth.Tool.LineConverterTool.ToVectorLines(lineData, prj);
        var polygonsNode = new RenderPolygonsNode();
        polygonsNode.ID = ID;
        polygonsNode.SetPolygons(polygons, 1f);
        return polygonsNode;
    }

    protected override void ReCreatePolygonsNode(LinesNode texture, LineData lineData, IProjection prj)
    {
        // 由于RenderPolygonsNode不继承自LinesNode，这里无法直接转换
        // 但我们可以通过其他方式实现，比如重新创建节点
        _layerNode = CreatePolygonsNode(ID, lineData, prj);
    }

    protected override ILayerNode? CreatePolygonsNode(string ID, List<LineData> orderedLineDataList, IProjection prj)
    {
        var allPolygons = new List<Vicold.Atmospex.Data.Vector.VectorLine>();
        foreach (var lineData in orderedLineDataList)
        {
            var polygons = Vicold.Atmospex.Earth.Tool.LineConverterTool.ToVectorLines(lineData, prj);
            allPolygons.AddRange(polygons);
        }
        var polygonsNode = new Vicold.Atmospex.Godot.Frame.Nodes.RenderPolygonsNode();
        polygonsNode.ID = ID;
        polygonsNode.SetPolygons(allPolygons.ToArray(), 1f);
        return polygonsNode;
    }

    protected override void ReCreatePolygonsNode(LinesNode texture, List<LineData> orderedLineDataList, IProjection prj)
    {
        // 由于RenderPolygonsNode不继承自LinesNode，这里无法直接转换
        // 但我们可以通过其他方式实现，比如重新创建节点
        _layerNode = CreatePolygonsNode(ID, orderedLineDataList, prj);
    }

    public void Draw(Node? parent = null)
    {
        if (_layerNode is IRenderNode node)
        {
            node.Draw(parent, LayerDescription);
        }
    }

    public void Erase()
    {
        if (_layerNode is IRenderNode node)
        {
            node.Erase();
        }
    }

    public override void ScaleChange(float scale)
    {
        var linesNode = _layerNode as Vicold.Atmospex.Godot.Frame.Nodes.RenderLinesNode;
        if (linesNode != null)
        {
            linesNode.ResetScale(scale);
            return;
        }
        var polygonsNode = _layerNode as Vicold.Atmospex.Godot.Frame.Nodes.RenderPolygonsNode;
        if (polygonsNode != null)
        {
            polygonsNode.ResetScale(scale);
            return;
        }
        var textureNode = _layerNode as Vicold.Atmospex.Godot.Frame.Nodes.RenderTextureNode;
        if (textureNode != null)
        {
            textureNode.ResetScale(scale);
        }
    }
}