using System.Drawing;
using HighContour;
using HighContour.Model;
using Vicold.Atmospex.Algorithm;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Style;

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
    protected abstract ILayerNode? CreatePolygonsNode(string ID, List<LineData> orderedLineDataList, IProjection prj);
    protected abstract void ReCreatePolygonsNode(LinesNode texture, List<LineData> orderedLineDataList, IProjection prj);

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
            var isCountour = Style.Palette.RenderType == RenderType.Contour;
            LayerZLevel = isCountour ? LayerLevel.High : LayerLevel.Low;

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


            if (isCountour)
            {
                var lineData = new LineData();
                if (contour is { })
                {
                    foreach (var lineContour in contour)
                    {
                        var width = lineContour.Value % 10 == 0 ? 2 : 1;
                        var listLine = Algorithm.GeographyAlgorithm.CrossLonBorderSplitLine(lineContour.LinePoints, 180);

                        foreach (var line2 in listLine)
                        {
                            var dataArrayTiles = LineTileAlgorithm.CreateTileLines(line2);
                            foreach (var tiled_line in dataArrayTiles)
                            {
                                var c = Style.Palette.Select(lineContour.Value);
                                lineData.Add(line2, tiled_line, width, lineContour.Value, Color.FromArgb(c.A, c.R, c.G, c.B), PolygonType.Line);
                            }
                        }
                    }
                }

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
                // 生成每个等值线对应的LineData
                var polygonLevelData = new Dictionary<float, LineData>();
                if (contour is { })
                {
                    foreach (var lineContour in contour)
                    {
                        var width = lineContour.Value % 10 == 0 ? 2 : 1;
                        var listLine = Algorithm.GeographyAlgorithm.CrossLonBorderSplitLine(lineContour.LinePoints, 180);
                        
                        // 根据lineContour.Value获取或创建对应的LineData
                        if (!polygonLevelData.TryGetValue(lineContour.Value, out var value))
                        {
                            value = new LineData();
                            polygonLevelData[lineContour.Value] = value;
                        }
                        
                        foreach (var line2 in listLine)
                        {
                            var c = Style.Palette.Select(lineContour.Value);
                            value.Add(line2, width, lineContour.Value, Color.FromArgb(c.A, c.R, c.G, c.B), PolygonType.Fill);
                        }
                    }
                }
                
                // 根据anas值的顺序将polygonLevelData字典转换为List<LineData>
                var orderedLineDataList = new List<LineData>();
                foreach (var value in anas)
                {
                    if (polygonLevelData.TryGetValue(value,out var p))
                    {
                        orderedLineDataList.Add(p);
                    }
                }

                

                if (_layerNode == null)
                {
                    var texture = CreatePolygonsNode(ID, orderedLineDataList, projection);
                    if (texture != null)
                    {
                        texture.SetLevel((int)LayerZLevel + ZIndex);
                    }

                    _layerNode = texture;
                }
                else
                {
                    ReCreatePolygonsNode((LinesNode)_layerNode, orderedLineDataList, projection);
                }
            }
        }
    }

}