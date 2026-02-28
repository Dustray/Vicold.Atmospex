using Godot;

namespace Vicold.Atmospex.Godot.Frame.Layers;

public interface IRenderLayer
{
    RenderLayerDescription LayerDescription { get; }

    void Draw(Node? parent = null);

    void Erase();
}

public class RenderLayerDescription
{
    public string Name { get; set; }
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;
}