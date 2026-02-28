using Godot;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Godot.Frame.Nodes;

namespace Vicold.Atmospex.Godot.Frame.Layers;

public class RenderLineLayer : LineLayer, IRenderLayer
{
    public RenderLineLayer(IVectorDataProvider provider, string id)
        : base(provider, id)
    {
        // 初始化层描述
        LayerDescription = new RenderLayerDescription
        {
            Name = "LineLayer",
            Order = 90, // 线层通常在底层
            IsVisible = true
        };
    }

    public RenderLayerDescription LayerDescription { get; private set; }

    protected override ILayerNode? CreateLinesNode(string ID, IVectorDataProvider provider, IProjection prj)
    {
        // 空实现，待后续填充
        return null;
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