using Evergine.Common;
using Evergine.Common.Graphics;
using Evergine.Common.Graphics.VertexFormats;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Graphics.Materials;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vicold.Atmospex.Algorithm;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Render.Frame.Tools;
namespace Vicold.Atmospex.Render.Frame.Models
{
    public class PolygonsModel
    {
        private readonly VectorLine[] _polygons;
        private readonly RenderLayerDescription _renderLayer;
        private readonly List<Entity> _modelEntities = [];
        private readonly GraphicsContext _graphicsContext;
        private readonly AssetsService _assetsService;

        public PolygonsModel(VectorLine[] polygons, RenderLayerDescription renderLayer)
        {
            _polygons = polygons;
            _renderLayer = renderLayer;
            _graphicsContext = Application.Current.Container.Resolve<GraphicsContext>();
            _assetsService = Application.Current.Container.Resolve<AssetsService>();
            InitializeModels();
        }

        private void InitializeModels()
        {
            if (_graphicsContext == null || _polygons.Length == 0)
                return;

            var effect = _assetsService.Load<Effect>(EvergineContent.Effects.StandardEffect);

            foreach (var polygon in _polygons)
            {
                if (polygon.Data.Length < 3)
                    continue;

                Mesh? mesh = CreatePolygonMesh(polygon);
                if(mesh is null)
                {
                    continue;
                }

                var material = new StandardMaterial(effect)
                {
                    VertexColorEnabled = true,
                    IBLEnabled = false,
                    LightingEnabled = false,
                    LayerDescription = _renderLayer,
                    Alpha = polygon.FillColor.A / 255f
                };

                var model = new Model("PolygonModel", mesh);
                var meshEntity = model.InstantiateModelHierarchy("Polygon", _assetsService);
                meshEntity.AddComponent(new MaterialComponent() { Material = material.Material });
                meshEntity.IsEnabled = true;
                _modelEntities.Add(meshEntity);
            }
        }

        private Mesh CreatePolygonMesh(VectorLine polygon)
        {
            // 1) 三角化（得到实际用于渲染的顶点集和索引）
            var tessResult = PolygonTessellatorAlgorithm.TessellateSimple(polygon.Data);
            if (tessResult.Indices.Length == 0 || tessResult.Positions.Length == 0)
                return null;

            // 如果顶点数超过 ushort.MaxValue，需要改用 32-bit 索引或拆分网格
            if (tessResult.Positions.Length > ushort.MaxValue)
                throw new InvalidOperationException("Polygon too large for 16-bit indices; split or use 32-bit indices.");

            // 2) 构造 VertexPositionColor[]（注意这里使用 tessResult.Positions）
            var color = new Color(polygon.FillColor.R, polygon.FillColor.G, polygon.FillColor.B, polygon.FillColor.A);
            var vertices = new VertexPositionColor[tessResult.Positions.Length];
            for (int i = 0; i < tessResult.Positions.Length; i++)
            {
                vertices[i].Position = tessResult.Positions[i].ToEver();
                vertices[i].Color = color;
            }

            // 3) indices 已经是 ushort[]
            var indices = tessResult.Indices;

            // 4) 创建缓冲区描述并上传（保持你原来的 BufferDescription 逻辑）
            var vertexBufferDesc = new BufferDescription(
                (uint)(vertices.Length * Unsafe.SizeOf<VertexPositionColor>()),
                BufferFlags.VertexBuffer | BufferFlags.ShaderResource,
                ResourceUsage.Default);

            var indexBufferDesc = new BufferDescription(
                (uint)(indices.Length * sizeof(ushort)),
                BufferFlags.IndexBuffer,
                ResourceUsage.Default);

            var vBuffer = _graphicsContext.Factory.CreateBuffer(vertices, ref vertexBufferDesc);
            var iBuffer = _graphicsContext.Factory.CreateBuffer(indices, ref indexBufferDesc);

            var vertexBuffer = new VertexBuffer(vBuffer, VertexPositionColor.VertexFormat);
            var indexBuffer = new IndexBuffer(iBuffer);

            return new Mesh(new[] { vertexBuffer }, indexBuffer, PrimitiveTopology.TriangleList)
            {
                BoundingBox = ComputeBoundingBox(tessResult.Positions),
                AllowBatching = true
            };
        }

        private BoundingBox ComputeBoundingBox(System.Numerics.Vector3[] points)
        {
            if (points == null || points.Length == 0) return new BoundingBox();
            Vector3 min = new(float.MaxValue);
            Vector3 max = new(float.MinValue);
            foreach (var p in points)
            {
                min = Vector3.Min(min, p.ToEver());
                max = Vector3.Max(max, p.ToEver());
            }
            return new BoundingBox(min, max);
        }

        public List<Entity> GetModelEntities() => _modelEntities;
    }
}