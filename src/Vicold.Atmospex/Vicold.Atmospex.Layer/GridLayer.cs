using HighContour;
using HighContour.Model;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Style;
using System.Drawing;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Layer.Node;

namespace Vicold.Atmospex.Layer;

public abstract class GridLayer : Layer
{
    public GridLayer(IGridDataProvider provider)
    {
        DataProvider = provider;
        LayerZLevel = LayerLevel.Low;
    }

    protected abstract ILayerNode? CreateTextureNode(string ID, GridDataProvider provider, IPalette palette, IProjection prj);
    protected abstract void ReCreateTextureNode(TextureNode texture, GridDataProvider provider, IPalette palette, IProjection prj);
    protected abstract ILayerNode? CreateLinesNode(string ID, IVectorDataProvider provider, IProjection prj);
    protected abstract void ReCreateLinesNode(LinesNode texture, IVectorDataProvider provider, IProjection prj);
    protected abstract ILayerNode? CreateLinesNode(string ID, LineData lineData, IProjection prj);
    protected abstract void ReCreateLinesNode(LinesNode texture, LineData lineData, IProjection prj);

    protected abstract ILayerNode? CreatePolygonsNode(string ID, IVectorDataProvider provider, IProjection prj);
    protected abstract void ReCreatePolygonsNode(LinesNode texture, IVectorDataProvider provider, IProjection prj);
    protected abstract ILayerNode? CreatePolygonsNode(string ID, LineData lineData, IProjection prj);
    protected abstract void ReCreatePolygonsNode(LinesNode texture, LineData lineData, IProjection prj);

    public override void Render(IProjection projection)
    {
        base.Render(projection);

        if (DataProvider is not GridDataProvider provider)
        {
            throw new Exception("未指定Provider");
        }


        //GridData data;
        //if (provider.Level >= provider.Count)
        //{
        //    data = provider.GetData();
        //}
        //else
        //{
        //    data = provider.GetData(provider.Level);
        //}

        if (Style == null)
        {
            return;
        }

        if (Style.Palette.RenderType == RenderType.Bitmap)
        {
            LayerZLevel = LayerLevel.Low;

            if (_layerNode != null && _layerNode is not TextureNode)
            {
                return;
            }

            if (_layerNode == null)
            {
                var texture = CreateTextureNode(ID, provider, Style.Palette, projection);
                if (texture != null)
                    texture.SetLevel((int)LayerZLevel + ZIndex);
                _layerNode = texture;
            }
            else
            {
                ReCreateTextureNode((TextureNode)_layerNode, provider, Style.Palette, projection);
            }
        }
        else if (Style.Palette.RenderType == RenderType.Contour || Style.Palette.RenderType == RenderType.Polygon)
        {
            LayerZLevel = LayerLevel.High;

            if (_layerNode != null && _layerNode is not LinesNode)
            {
                return;
            }

            float[] anas;
            if (Style.Palette.UseAnaValues && Style.Palette.ContourAnaValues != null)
            {
                anas = Style.Palette.ContourAnaValues;
            }
            else
            {
                anas = new float[Style.Palette.Count];
                for (var i = 0; i < Style.Palette.Count; i++)
                {
                    anas[i] = Style.Palette.Get(i).Value;
                }
            }

            provider.LoadData();
            var d = provider.GetData();
            Contour[]? contour = null;
            d.ScopeLock((it) =>
            {
                var op = new ContourOptions()
                {
                    Longitude = (float)provider.StartLongitude,
                    Latitude = (float)provider.StartLatitude,
                    LonInterval = (float)provider.LonInterval,
                    LatInterval = (float)provider.LatInterval,
                };
                contour = ContourCreator.Excute(it, 0, 0, d.Width, d.Height, d.Width, anas, op);
            });

            if (provider.SmoothType != -1)
            {
                d.Dispose();
            }

            var lineData = new LineData();

            if (contour is { })
            {
                foreach (var line in contour)
                {
                    var width = line.Value % 10 == 0 ? 2 : 1;
                    var listLine = Algorithm.GeographyAlgorithm.CrossLonBorderSplitLine(line.LinePoints, 180);
                    foreach (var line2 in listLine)
                    {
                        var c = Style.Palette.Select(line.Value);
                        lineData.Add(line2, width, line.Value, Color.FromArgb(c.A, c.R, c.G, c.B));
                    }
                }
            }

            if (Style.Palette.RenderType == RenderType.Contour)
            {
                if (_layerNode == null)
                {
                    var texture = CreateLinesNode(ID, lineData, projection);
                    if (texture != null)
                    {
                        texture.SetLevel((int)LayerZLevel + ZIndex);
                    }

                    _layerNode = texture;
                }
                else
                {
                    ReCreateLinesNode((LinesNode)_layerNode, lineData, projection);
                }
            }
            else
            {
                if (_layerNode == null)
                {
                    var texture = CreatePolygonsNode(ID, lineData, projection);
                    if (texture != null)
                    {
                        texture.SetLevel((int)LayerZLevel + ZIndex);
                    }

                    _layerNode = texture;
                }
                else
                {
                    ReCreatePolygonsNode((LinesNode)_layerNode, lineData, projection);
                }
            }
        }
    }
}