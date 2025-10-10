using Evergine.Common.Graphics;
using Evergine.Framework;
using Evergine.Framework.Managers;
using Evergine.Mathematics;
using System.Collections.Generic;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Render.Frame.Models;
namespace Vicold.Atmospex.Render.Frame.Nodes
{
    internal sealed class RenderPolygonsNode : LinesNode, IRenderNode
    {
        private readonly RenderLayerDescription _renderLayer;
        private Entity? _polygonEntity;
        private List<Entity> _childrenEntities = [];

        private PolygonsModel? LevelLinesModel;
        public RenderPolygonsNode(List<VectorLine[]> polygons, RenderLayerDescription renderLayer) : base(polygons)
        {
            _renderLayer = renderLayer;
            _polygonEntity = null;
        }

        public int ZIndex { get; set; }

        public bool IsTileEnabled { get; set; } = true;

        public override bool Visible
        {
            get => _polygonEntity?.IsEnabled ?? false; set
            {
                if (_polygonEntity is { })
                {
                    _polygonEntity.IsEnabled = value;
                }
            }
        }

        public override void SetLevel(int zIndex)
        {
            ZIndex = zIndex;
            _renderLayer.Order = ZIndex;
        }

        public void Draw(EntityManager entityManager, RenderLayerDescription layerDescription)
        {
            if (_polygonEntity == null)
            {
                // 创建空Entity，不添加任何组件
                _polygonEntity = new Entity();
                // 添加到实体管理器
                entityManager.Add(_polygonEntity);
                // 创建PolygonsModel
                LevelLinesModel = new PolygonsModel(LevelLines, _renderLayer);
                // 获取模型实体并添加到空Entity中
                var modelEntities = LevelLinesModel.GetModelEntities();
                foreach (var entity in modelEntities)
                {
                    _polygonEntity.AddChild(entity);
                    _childrenEntities.Add(entity);
                }
            }
        }

        public void Erase(EntityManager entityManager)
        {
            if (_polygonEntity is { })
            {
                foreach (var c in _childrenEntities)
                {
                    entityManager.Detach(c);
                    c.Destroy();
                }
                _childrenEntities.Clear();

                entityManager.Detach(_polygonEntity);
                _polygonEntity.Destroy();
                _polygonEntity = null;
            }
        }

        public override void Dispose()
        {
            _childrenEntities.Clear();
        }
    }
}