using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Managers;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.DataService.Provider;
using Vicold.Atmospex.Earth.ImageMaker;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Earth.Tool;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Render.Frame.Models;
using Vicold.Atmospex.Render.Frame.Nodes;
using Vicold.Atmospex.Style;

namespace Vicold.Atmospex.Render.Frame.Layers
{
    public class RenderGridLayer : GridLayer, IRenderLayer
    {
        private readonly GraphicsContext _graphicsContext;

        public RenderGridLayer(IGridDataProvider provider, GraphicsContext graphicsContext) : base(provider)
        {
            _graphicsContext = graphicsContext;

            // Create a custom RenderLayer with specified render states
            LayerDescription = new()
            {
                RenderState = new RenderStateDescription()
                {
                    RasterizerState = new RasterizerStateDescription()
                    {
                        CullMode = CullMode.Back,
                        FillMode = FillMode.Solid,
                    },
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.ReadWrite,
                },
                Order = -120,
                SortMode = SortMode.FrontToBack,
            };
        }

        public RenderLayerDescription LayerDescription { get; private set; }

        // 创建纹理图像
        private Texture? RenderImage(GridDataProvider provider, GridData data, IProjection prj, ImageBound bound, IPalette palette)
        {
            if (data == null)
            {
                return null;
            }

            // 计算世界坐标尺寸和图像尺寸
            double world_height = (bound.MaxY - bound.MinY);
            double world_width = (bound.MaxX - bound.MinX);

            int bmp_width = bound.XSize;
            double aspect_ratio = world_height / world_width;
            int bmp_height = (int)(aspect_ratio * bmp_width);

            // 创建纹理描述
            var description = new TextureDescription()
            {
                Type = TextureType.Texture2D,
                Width = (uint)bmp_width,
                Height = (uint)bmp_height,
                Depth = 1,
                ArraySize = 1,
                Faces = 1,
                Usage = ResourceUsage.Default,
                CpuAccess = ResourceCpuAccess.None,
                Flags = TextureFlags.ShaderResource,
                Format = PixelFormat.R32G32B32A32_Float,
                MipLevels = 1,
                SampleCount = TextureSampleCount.None,
            };

            // 生成图像数据
            float[] imageData = GenerateImageData(provider, data, prj, bound, palette, bmp_width, bmp_height);

            // 计算行间距和切片间距
            var rowPitch = Evergine.Common.Graphics.Helpers.GetRowPitch((uint)bmp_width, PixelFormat.R32G32B32A32_Float);
            var slicePitch = Evergine.Common.Graphics.Helpers.GetSlicePitch(rowPitch, (uint)bmp_height, PixelFormat.R32G32B32A32_Float);

            // 固定数据以便获取指针
            var pinnedHandle = GCHandle.Alloc(imageData, GCHandleType.Pinned);
            IntPtr dataPointer = Marshal.UnsafeAddrOfPinnedArrayElement(imageData, 0);
            var databox = new DataBox[] { new(dataPointer, rowPitch, slicePitch) };

            // 创建纹理
            var texture = this._graphicsContext.Factory.CreateTexture(databox, ref description);

            // 释放固定的句柄
            pinnedHandle.Free();

            return texture;
        }

        // 生成图像数据
        private static float[] GenerateImageData(GridDataProvider provider, GridData data, IProjection prj, ImageBound bound, IPalette palette, int width, int height)
        {
            float[] dataArray = new float[width * height * 4]; // RGBA格式

            double world_height = (bound.MaxY - bound.MinY);
            double world_width = (bound.MaxX - bound.MinX);
            double xResolution = world_width / (width - 1);
            double yResolution = world_height / (height - 1);

            int endY = bound.YOffset + bound.YSize - 1;

            // 为了把右半部分挪到左边
            var cutX = width;
            if (bound.East < provider.EndLongitude)
            {
                var ra = bound.East / provider.EndLongitude;
                cutX = (int)(ra * width);
            }

            // 使用并行处理提高性能
            Parallel.For(0, height, y =>
            {
                double xx = 0;
                double yy = bound.MaxY - y * yResolution;
                prj.Index2Geo(xx, yy, out double lon, out double lat);
                int row = -1;
                if (bound.LatInterval > 0)
                {
                    row = (int)((lat - bound.South) / bound.LatInterval);
                }
                else
                {
                    row = (int)((lat - bound.North) / bound.LatInterval);
                }

                var yS = bound.LatInterval < 0 ? height - y - 1 : y;

                if (row >= bound.YOffset && row <= endY)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var xS = x;
                        if (cutX != width)
                        {
                            // 数据和地图经纬度位置不同时左右互换
                            xS = x >= cutX ? x - cutX : x + width - cutX;
                        }

                        int col = x + bound.XOffset;
                        var ci = data.ReadFloat(col, row);
                        Color color;
                        if (Math.Abs(ci - data.NoDataValue ?? 0) < 1e-6)
                        {
                            color = Color.Transparent;
                        }
                        else
                        {
                            ColorItem colorItem = palette.Select(ci);
                            color = new Color(colorItem.R, colorItem.G, colorItem.B, colorItem.A);
                        }

                        int index = (yS * width + xS) * 4;
                        dataArray[index] = color.R / 255.0f;
                        dataArray[index + 1] = color.G / 255.0f;
                        dataArray[index + 2] = color.B / 255.0f;
                        dataArray[index + 3] = color.A / 255.0f;
                    }
                }
            });

            return dataArray;
        }

        protected override ILayerNode? CreateLinesNode(string ID, IVectorDataProvider provider, IProjection prj)
        {
            var vectorData = provider.GetData();
            if (vectorData is LineData lineData)
            {
                var lines = LineConverterTool.ToVectorLines(lineData, prj);
                var linesNode = new RenderLinesNode(lines, LayerDescription)
                {
                    ID = ID
                };
                return linesNode;
            }

            return null;
        }

        protected override ILayerNode? CreateLinesNode(string ID, LineData lineData, IProjection prj)
        {
            var lines = LineConverterTool.ToVectorLines(lineData, prj);
            var linesNode = new RenderLinesNode(lines, LayerDescription)
            {
                ID = ID
            };
            return linesNode;
        }

        protected override ILayerNode? CreatePolygonsNode(string ID, IVectorDataProvider provider, IProjection prj)
        {
            var vectorData = provider.GetData();
            if (vectorData is LineData lineData)
            {
                var polygons = LineConverterTool.ToVectorLines(lineData, prj);
                var polygonsNode = new RenderPolygonsNode(polygons, LayerDescription)
                {
                    ID = ID
                };
                return polygonsNode;
            }

            return null;
        }

        protected override ILayerNode? CreatePolygonsNode(string ID, LineData lineData, IProjection prj)
        {
            var polygons = LineConverterTool.ToVectorLines(lineData, prj);
            var polygonsNode = new RenderPolygonsNode(polygons, LayerDescription)
            {
                ID = ID
            };
            return polygonsNode;
        }

        protected override ILayerNode? CreateTextureNode(string ID, GridDataProvider provider, IPalette palette, IProjection prj)
        {
            provider.LoadData();
            GridData data = provider.GetData();
            var bound = EMImage.CalculateBound(provider, data, prj);
            var tx = RenderImage(provider, data, prj, bound, palette);
            if (tx is not { }) { return null; }
            var texture = new RenderTextureNode(_graphicsContext)
            {
                TImage = tx,
                ID = ID,
                StartX = (float)bound.MinX,
                StartY = (float)bound.MinY,
                WorldWidth = (float)(bound.MaxX - bound.MinX),
                WorldHeight = (float)(bound.MaxY - bound.MinY)
            };

            if (provider.SmoothType != -1)
            {
                data.Dispose();
            }
            return texture;
        }

        protected override void ReCreateLinesNode(LinesNode linesNode, IVectorDataProvider provider, IProjection prj)
        {
            if (linesNode is RenderLinesNode renderLinesNode)
            {
                var vectorData = provider.GetData();
                if (vectorData is LineData lineData)
                {
                    var lines = LineConverterTool.ToVectorLines(lineData, prj);
                    linesNode.ResetLines(lines);
                }
            }
        }

        protected override void ReCreateLinesNode(LinesNode texture, LineData lineData, IProjection prj)
        {
            if (texture is RenderLinesNode renderLinesNode)
            {
                var lines = LineConverterTool.ToVectorLines(lineData, prj);
                texture.ResetLines(lines);
            }
        }

        protected override void ReCreateTextureNode(TextureNode texture, GridDataProvider provider, IPalette palette, IProjection prj)
        {
            provider.LoadData();
            GridData data = provider.GetData();
            var bound = EMImage.CalculateBound(provider, data, prj);
            var newImage = RenderImage(provider, data, prj, bound, palette);
            if (provider.SmoothType != -1)
            {
                data.Dispose();
            }
            if (texture is RenderTextureNode renderTexture && newImage is { })
            {
                renderTexture.ResetImage(newImage);
            }
        }

        protected override void ReCreatePolygonsNode(LinesNode polygonsNode, IVectorDataProvider provider, IProjection prj)
        {
            if (polygonsNode is RenderPolygonsNode renderPolygonsNode)
            {
                var vectorData = provider.GetData();
                if (vectorData is LineData lineData)
                {
                    var polygons = LineConverterTool.ToVectorLines(lineData, prj);
                    polygonsNode.ResetLines(polygons);
                }
            }
        }

        protected override void ReCreatePolygonsNode(LinesNode polygonsNode, LineData lineData, IProjection prj)
        {
            if (polygonsNode is RenderPolygonsNode renderPolygonsNode)
            {
                var polygons = LineConverterTool.ToVectorLines(lineData, prj);
                polygonsNode.ResetLines(polygons);
            }
        }

        public void Draw(EntityManager entityManager)
        {
            if (_layerNode is IRenderNode node)
            {
                node.Draw(entityManager, LayerDescription);
            }
        }
    }
}
