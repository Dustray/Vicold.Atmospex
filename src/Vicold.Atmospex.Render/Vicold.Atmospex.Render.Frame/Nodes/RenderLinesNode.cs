using Evergine.Common.Graphics;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Render.Components;

namespace Vicold.Atmospex.Render.Frame.Nodes
{
    internal class RenderLinesNode : LinesNode, IRenderNode
    {
        private EverBatchLine2? _everBatchLine;
        private readonly VectorLine[] _lines;
        private RenderLayerDescription _renderLayer;
        private Entity? _batchLineEntity;

        /// <summary>
        /// 控制是否使用贝塞尔曲线的开关，默认关闭
        /// </summary>
        public bool UseBezierCurve { get; set; } = false;

        public bool IsTileEnabled { get; set; } = true;

        public bool IsLineRarefyEnabled { get; set; } = true;

        public RenderLinesNode(VectorLine[] lines, RenderLayerDescription renderLayer) : base(lines)
        {
            _lines = lines;
            _renderLayer = renderLayer;
            _batchLineEntity = null;
        }

        private void InitializeEverBatchLine()
        {
            // 创建EverBatchLine2
            _everBatchLine = new EverBatchLine2(_renderLayer, IsTileEnabled, IsLineRarefyEnabled)
            {
                Lines = _lines,
                UseBezierCurve = this.UseBezierCurve
            };

            // 计算线条厚度
            float averageWidth = _lines.Average(l => l.Width);
            _everBatchLine.LineThickness = averageWidth > 0 ? averageWidth / 1000 : 0.01f;
        }

        //private void UpdateEverBatchLine()
        //{
        //    if (_everBatchLine == null || _lines == null || _lines.Length == 0)
        //        return;

        //    _everBatchLine.Lines = _lines;
        //    _everBatchLine.UseBezierCurve = this.UseBezierCurve;

        //    // 计算线条厚度
        //    float averageWidth = _lines.Average(l => l.Width);
        //    _everBatchLine.LineThickness = averageWidth > 0 ? averageWidth / 1000 : 0.01f;

        //    // 更新线条
        //    _everBatchLine.UpdateLines();
        //}

        // 贝塞尔曲线计算辅助方法
        private static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }
        public override void SetLevel(int zIndex)
        {
            ZIndex = zIndex;
            // LineBatch3D doesn't have a direct ZIndex property, but we can adjust the Z position
            // This would require modifying the line positions, which we can do in UpdateLineBatch
            _renderLayer.Order = ZIndex;
        }

        public int ZIndex { get; set; }
        public override bool Visible
        {
            get => _batchLineEntity?.IsEnabled ?? false; set
            {
                if (_batchLineEntity is { })
                {
                    _batchLineEntity.IsEnabled = value;
                }
            }
        }

        public void Draw(EntityManager entityManager, RenderLayerDescription layerDescription)
        {
            if (_batchLineEntity == null)
            {
                // 初始化EverBatchLine
                InitializeEverBatchLine();

                var tectTrans = new Transform3D
                {
                    Position = new Vector3(0, 0, 0),
                    Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi) // 无需旋转，默认面向 Z+
                };
                // 创建实体并添加组件
                _batchLineEntity = new Entity()
                    .AddComponent(tectTrans)
                    .AddComponent(_everBatchLine);

                // 添加到实体管理器
                entityManager.Add(_batchLineEntity);
            }
            else
            {
                // 更新线条
                //UpdateEverBatchLine();
            }
        }


        public void Erase(EntityManager entityManager)
        {
            if (_batchLineEntity is { })
            {
                entityManager.Detach(_batchLineEntity);
                _batchLineEntity.Destroy();
                _batchLineEntity = null;
            }
        }

        public override void Dispose()
        {
        }
    }
}
