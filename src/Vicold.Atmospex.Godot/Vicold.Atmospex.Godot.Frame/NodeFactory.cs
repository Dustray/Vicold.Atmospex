using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Earth.Tool;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Style;
using Vicold.Atmospex.Godot.Frame.Nodes;

namespace Vicold.Atmospex.Godot.Frame;

public class NodeFactory
{
    public static ILayerNode CreateFontNode(string ID, IVectorDataProvider provider, IProjection projection)
    {
        var vectorData = provider.GetData();
        if (vectorData is VectorData vector)
        {
            IList<VectorFont> vectorFonts = new List<VectorFont>();
            var count = vector.Collection.Count;
            for (var i = 0; i < count; i++)
            {
                var vectorElement = vector.Collection[i];
                if (vectorElement.Type == VectorType.Font)
                {
                    if (vectorElement is FontElement fontElement)
                    {
                        var position = fontElement.Position;
                        projection.Geo2Index(position.X, position.Y, out var x, out var y);
                        var offset = fontElement.Offset;
                        vectorFonts.Add(new VectorFont()
                        {
                            Font = (string)fontElement.Data,
                            Position = new System.Numerics.Vector2((float)(x + offset.X), (float)(y + offset.Y)),
                            Pivot = new System.Numerics.Vector2(fontElement.Pivot.X, fontElement.Pivot.Y),
                            FontColor = fontElement.FontColor,
                            FontSize = fontElement.FontSize,
                        });
                    }
                }
            }

            var fontNode = new RenderFontNode();
            fontNode.SetFonts(vectorFonts);
            fontNode.ID = ID;
            return fontNode;
        }

        return null;
    }

    public static ILayerNode CreateTextureNode(string ID, GridDataProvider provider, IPalette palette, IProjection projection)
    {
        provider.LoadData();
        var data = provider.GetData();
        var textureNode = new RenderTextureNode();
        textureNode.SetTexture(provider, palette);
        textureNode.ID = ID;
        return textureNode;
    }

    public static void ReCreateTextureNode(RenderTextureNode texture, GridDataProvider provider, IPalette palette, IProjection projection)
    {
        provider.LoadData();
        texture.ResetImage(provider, palette);
    }

    public static ILayerNode CreateLinesNode(string ID, IVectorDataProvider provider, IProjection projection)
    {
        var vectorData = provider.GetData();
        if (vectorData is LineData lineData)
        {
            var lines = LineConverterTool.ToVectorLines(lineData, projection);
            var linesNode = new RenderLinesNode();
            linesNode.SetLines(lines, 1f);
            linesNode.ID = ID;
            return linesNode;
        }

        return null;
    }

    public static ILayerNode CreateLinesNode(string ID, LineData lineData, IProjection projection)
    {
        var lines = LineConverterTool.ToVectorLines(lineData, projection);
        var linesNode = new RenderLinesNode();
        linesNode.SetLines(lines, 1f);
        linesNode.ID = ID;
        return linesNode;
    }

    public static void ReCreateLinesNode(RenderLinesNode texture, IVectorDataProvider provider, IProjection projection)
    {
        var vectorData = provider.GetData();
        if (vectorData is LineData lineData)
        {
            var lines = LineConverterTool.ToVectorLines(lineData, projection);
            texture.ResetLines(lines);
        }
    }

    public static void ReCreateLinesNode(RenderLinesNode texture, LineData lineData, IProjection projection)
    {
        var lines = LineConverterTool.ToVectorLines(lineData, projection);
        texture.ResetLines(lines);
    }

    public static ILayerNode CreatePolygonsNode(string ID, IVectorDataProvider provider, IProjection projection)
    {
        var vectorData = provider.GetData();
        if (vectorData is LineData lineData)
        {
            var polygons = LineConverterTool.ToVectorLines(lineData, projection);
            var polygonsNode = new RenderPolygonsNode();
            polygonsNode.SetPolygons(polygons, 1f);
            polygonsNode.ID = ID;
            return polygonsNode;
        }

        return null;
    }

    public static ILayerNode CreatePolygonsNode(string ID, LineData lineData, IProjection projection)
    {
        var polygons = LineConverterTool.ToVectorLines(lineData, projection);
        var polygonsNode = new RenderPolygonsNode();
        polygonsNode.SetPolygons(polygons, 1f);
        polygonsNode.ID = ID;
        return polygonsNode;
    }

    public static void ReCreatePolygonsNode(RenderPolygonsNode texture, IVectorDataProvider provider, IProjection projection)
    {
        var vectorData = provider.GetData();
        if (vectorData is LineData lineData)
        {
            var polygons = LineConverterTool.ToVectorLines(lineData, projection);
            texture.ResetPolygons(polygons);
        }
    }

    public static void ReCreatePolygonsNode(RenderPolygonsNode texture, LineData lineData, IProjection projection)
    {
        var polygons = LineConverterTool.ToVectorLines(lineData, projection);
        texture.ResetPolygons(polygons);
    }
}
