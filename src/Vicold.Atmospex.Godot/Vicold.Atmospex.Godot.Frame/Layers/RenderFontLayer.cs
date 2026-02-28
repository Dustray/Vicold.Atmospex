using Godot;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Godot.Frame.Nodes;

namespace Vicold.Atmospex.Godot.Frame.Layers;

public class RenderFontLayer : FontLayer, IRenderLayer
{
    public RenderFontLayer(IVectorDataProvider provider, string id)
        : base(provider, id)
    {
        LayerDescription = new RenderLayerDescription
        {
            Name = "FontLayer",
            Order = 110,
            IsVisible = true
        };
    }

    public RenderLayerDescription LayerDescription { get; private set; }

    protected override ILayerNode? CreateVectorsNode(string ID, IVectorDataProvider provider, IProjection proj)
    {
        return NodeFactory.CreateFontNode(ID, provider, proj);
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
        if (_layerNode is IRenderNode node)
        {
            node.ResetScale(scale);
        }
    }
}
