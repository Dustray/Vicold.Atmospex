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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vicold.Atmospex.Data.Vector;
namespace Vicold.Atmospex.Render.Frame.Models
{
    public class PolygonsModel
    {
        private readonly VectorLine[] _polygons;
        private readonly RenderLayerDescription _renderLayer;
        private readonly List<Entity> _modelEntities = [];
        private GraphicsContext _graphicsContext;
        private AssetsService _assetsService;
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
            if (_graphicsContext == null || _polygons.Length == 0) return;
            // 创建material            
            var effect = _assetsService.Load<Effect>(EvergineContent.Effects.StandardEffect);
            foreach (var polygon in _polygons)
            {
                if (polygon.Data.Length < 3) continue;
                // 需要至少3个点来形成多边形       
                // 创建多边形网格 
                Mesh mesh = CreatePolygonMesh(polygon); var material = new StandardMaterial(effect)
                {
                    VertexColorEnabled = true,
                    IBLEnabled = false,
                    LightingEnabled = false,
                    LayerDescription = _renderLayer,
                    Alpha = polygon.FillColor.A / 255f,
                    // 确保Alpha值正确设置        
                };
                // 通过Model关联Mesh和MeshRenderer   
                var model = new Model("PolygonModel", mesh);
                var meshEntity = model.InstantiateModelHierarchy("Polygon", _assetsService);
                meshEntity.AddComponent(new MaterialComponent() { Material = material.Material });
                meshEntity.IsEnabled = true;
                _modelEntities.Add(meshEntity);
            }
        }
        private Mesh CreatePolygonMesh(VectorLine polygon)
        {
            // 创建顶点和索引数据
            int vertexCount = polygon.Data.Length; int indexCount = (vertexCount - 2) * 3;
            // 多边形中的三角形数量    
            // 创建顶点缓冲区
            var vertexBufferDescription = new BufferDescription((uint)(vertexCount * (uint)Unsafe.SizeOf<VertexPositionColor>()), BufferFlags.VertexBuffer | BufferFlags.ShaderResource, ResourceUsage.Default);
            var vertices = new VertexPositionColor[vertexCount];
            // 填充顶点数据
            var color = new Color(polygon.FillColor.R, polygon.FillColor.G, polygon.FillColor.B, polygon.FillColor.A);
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i].Position = new Vector3(polygon.Data[i].X, polygon.Data[i].Y, 0);
                vertices[i].Color = color;
            }
            // 创建索引缓冲区
            var indexBufferDescription = new BufferDescription((uint)(indexCount * sizeof(ushort)), BufferFlags.IndexBuffer, ResourceUsage.Default);
            var indices = new ushort[indexCount];            // 使用扇形技术创建三角形索引
            for (int i = 0; i < vertexCount - 2; i++)
            {
                indices[i * 3] = 0;
                // 中心点
                indices[i * 3 + 1] = (ushort)(i + 1); indices[i * 3 + 2] = (ushort)(i + 2);
            }
            // 创建缓冲区
            var vBuffer = _graphicsContext.Factory.CreateBuffer(vertices, ref vertexBufferDescription);
            var iBuffer = _graphicsContext.Factory.CreateBuffer(indices, ref indexBufferDescription);
            var vertexBuffer = new VertexBuffer(vBuffer, VertexPositionColor.VertexFormat);
            var indexBuffer = new IndexBuffer(iBuffer);
            // 创建网格
            return new Mesh([vertexBuffer], indexBuffer, PrimitiveTopology.TriangleList)
            {
                BoundingBox = ComputeBoundingBox(polygon.Data),
                AllowBatching = true
            };
        }

        private BoundingBox ComputeBoundingBox(System.Numerics.Vector2[] points)
        {
            if (points == null || points.Length == 0) return new BoundingBox();
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var point in points)
            {
                Vector3 p = new Vector3(point.X, point.Y, 0);
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }
            return new BoundingBox(min, max);
        }

        // 获取模型实体的方法
        public List<Entity> GetModelEntities()
        {
            return _modelEntities;
        }
    }
}