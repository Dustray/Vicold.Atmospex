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
        // 空实现，待后续填充
        return null;
    }

    protected override void ReCreateTextureNode(TextureNode texture, GridDataProvider provider, IPalette palette, IProjection prj)
    {
        // 空实现，待后续填充
    }

    protected override ILayerNode? CreateLinesNode(string ID, IVectorDataProvider provider, IProjection prj)
    {
        // 空实现，待后续填充
        return null;
    }

    protected override void ReCreateLinesNode(LinesNode texture, IVectorDataProvider provider, IProjection prj)
    {
        // 空实现，待后续填充
    }

    protected override ILayerNode? CreateLinesNode(string ID, LineData lineData, IProjection prj)
    {
        // 空实现，待后续填充
        return null;
    }

    protected override void ReCreateLinesNode(LinesNode texture, LineData lineData, IProjection prj)
    {
        // 空实现，待后续填充
    }

    protected override ILayerNode? CreatePolygonsNode(string ID, IVectorDataProvider provider, IProjection prj)
    {
        // 空实现，待后续填充
        return null;
    }

    protected override void ReCreatePolygonsNode(LinesNode texture, IVectorDataProvider provider, IProjection prj)
    {
        // 空实现，待后续填充
    }

    protected override ILayerNode? CreatePolygonsNode(string ID, LineData lineData, IProjection prj)
    {
        // 空实现，待后续填充
        return null;
    }

    protected override void ReCreatePolygonsNode(LinesNode texture, LineData lineData, IProjection prj)
    {
        // 空实现，待后续填充
    }

    protected override ILayerNode? CreatePolygonsNode(string ID, List<LineData> orderedLineDataList, IProjection prj)
    {
        // 空实现，待后续填充
        return null;
    }

    protected override void ReCreatePolygonsNode(LinesNode texture, List<LineData> orderedLineDataList, IProjection prj)
    {
        // 空实现，待后续填充
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
        // 空实现，待后续填充
    }
}