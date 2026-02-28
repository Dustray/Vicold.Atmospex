using Godot;
using Vicold.Atmospex.Godot.Frame.Layers;
using Vicold.Atmospex.Layer;

namespace Vicold.Atmospex.Godot.Frame.Nodes;

public interface IRenderNode : ILayerNode
{
    public bool IsTileEnabled { get; set; }

    void Draw(Node? parent = null, RenderLayerDescription layerDescription = null);

    void Erase();
}