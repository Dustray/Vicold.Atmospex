using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Evergine.Common.Graphics.VertexFormats;
using Evergine.Common.Graphics;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;
using Vicold.Atmospex.Render.Helpers;
using Evergine.Framework;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Graphics.Materials;
using Evergine.Framework.Services;
using Evergine.Components.Graphics3D;

namespace Vicold.Atmospex.Render.Models;
public class RectangleModel(float x, float y, float z, float width, float height)
{
    public float X
    {
        get;
    } = x;
    public float Y
    {
        get;
    } = y;
    public float Z
    {
        get;
    } = z;
    public float Width
    {
        get;
    } = width;
    public float Height
    {
        get;
    } = height;

    public Color[] VertexColors
    {
        get; set;
    } = new Color[4];

    public void SetVertexColors(string color)
    {
        SetVertexColors(color, color, color, color);
    }
    public void SetVertexColors(string top, string bottom)
    {
        SetVertexColors(top, top, bottom, bottom);
    }
    public void SetVertexColors(string topLeft, string topRight, string bottomRight, string bottomLeft)
    {
        SetVertexColors(Color.FromHex(topLeft), Color.FromHex(topRight), Color.FromHex(bottomRight), Color.FromHex(bottomLeft));
    }
    public void SetVertexColors(Color color)
    {
        SetVertexColors(color, color, color, color);
    }
    public void SetVertexColors(Color top, Color bottom)
    {
        SetVertexColors(top, top, bottom, bottom);
    }
    public void SetVertexColors(Color topLeft, Color topRight, Color bottomRight, Color bottomLeft)
    {
        VertexColors[0] = topLeft;
        VertexColors[1] = topRight;
        VertexColors[2] = bottomRight;
        VertexColors[3] = bottomLeft;
    }

    public Mesh CreateMesh()
    {
        // Vertices and indices data.
        var graphicsContext = Application.Current.Container.Resolve<GraphicsContext>();
        var indexData = new ushort[] { 0, 3, 2, 0, 2, 1 };

        var vertexData = new VertexPositionColor[]
        {
            new(new Vector3(X, Y, Z).ToChangeOverlook(), VertexColors[0]),
            new(new Vector3(X+Width, Y, Z).ToChangeOverlook(), VertexColors[1]),
            new(new Vector3(X+Width,Y+Height, Z).ToChangeOverlook(), VertexColors[2]),
            new(new Vector3(X, Y+Height, Z).ToChangeOverlook(), VertexColors[3]),
        };

        // Vertex Buffer
        var vBufferDescription = new BufferDescription()
        {
            SizeInBytes = (uint)Unsafe.SizeOf<VertexPositionColor>() * (uint)vertexData.Length,
            Flags = BufferFlags.ShaderResource | BufferFlags.VertexBuffer,
            Usage = ResourceUsage.Default
        };

        // We create the buffer using the vertex array data previously defined.
        var vBuffer = graphicsContext.Factory.CreateBuffer(vertexData, ref vBufferDescription);
        var vertexBuffer = new VertexBuffer(vBuffer, VertexPositionColor.VertexFormat);

        // Index Buffer
        var iBufferDescription = new BufferDescription()
        {
            SizeInBytes = (uint)(sizeof(ushort) * indexData.Length),
            Flags = BufferFlags.IndexBuffer,
            Usage = ResourceUsage.Default,
        };

        // We create the buffer using the ushort array data previously defined.
        var iBuffer = graphicsContext.Factory.CreateBuffer(indexData, ref iBufferDescription);
        var indexBuffer = new IndexBuffer(iBuffer);

        // Create Mesh using the previously defined vertex buffer and index buffer.
        return new Mesh([vertexBuffer], indexBuffer, PrimitiveTopology.TriangleList)
        {
            BoundingBox = ComputeBoundingBox(),
        };
    }

    private BoundingBox ComputeBoundingBox() => new(new Vector3(X, Y, Z).ToChangeOverlook(), new Vector3(X + Width, Y + Height, Z).ToChangeOverlook());

    public Entity ToEntity()
    {

        // We can create a mesh 
        Mesh mesh = this.CreateMesh();

        var assetsService = Application.Current.Container.Resolve<AssetsService>();
        // Load the effect
        Effect standardEffect = assetsService.Load<Effect>(EvergineContent.Effects.StandardEffect);
        RenderLayerDescription layer = assetsService.Load<RenderLayerDescription>(EvergineContent.RenderLayers.Opaque);
        var smaterial = new StandardMaterial(standardEffect)
        {

            VertexColorEnabled = true,
            IBLEnabled = false,
            LightingEnabled = false,
            LayerDescription = layer,
        };
        layer.Order = 1;

        // We simply pass the mesh as constructor parameter with the name.
        Model model = new Model("CustomModel", mesh);
        // We convert to  an entity as a normal model.
        var entity = model.InstantiateModelHierarchy("MyModel", assetsService);
        entity.AddComponent(new MaterialComponent() { Material = smaterial.Material });
        return entity;
    }
}
