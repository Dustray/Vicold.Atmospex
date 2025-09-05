using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Managers;
using Evergine.Framework.Runtimes;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Layer.Node;
using Vicold.Atmospex.Render.Frame.Meshes;

namespace Vicold.Atmospex.Render.Frame.Nodes
{
    internal class RenderLinesNode : LinesNode, IRenderNode
    {
        private GenericLineMesh _lineMesh = new();
        private VectorLine[] _lines;
        //private Texture? _context; 

        /// <summary>
        /// 控制是否使用贝塞尔曲线的开关，默认关闭
        /// </summary>
        public bool UseBezierCurve { get; set; } = false;

        public RenderLinesNode(VectorLine[] lines) : base(lines)
        {
            _lines = lines;
            InitializeLineMesh();
        }

        private void InitializeLineMesh()
        {
            if (_lines == null || _lines.Length == 0)
                return;

            // 收集所有线条的点作为控制点
            List<Vector3> controlPoints = new List<Vector3>();
            List<GenericLineMesh.GenericLinePatch> patches = new List<GenericLineMesh.GenericLinePatch>();

            // 每个补丁使用的控制点数量
            const int controlPointsPerPatch = 16;
            // 索引最大值（ushort的最大值）
            const int maxIndexValue = 65535;
            // 每个补丁在Build方法中被处理的次数（2-4次，这里取最大值4）
            const int patchProcessingCount = 4;
            // 每个补丁在tessellation=4时生成的顶点数
            const int verticesPerPatch = (4 + 1) * (4 + 1); // 25

            foreach (var line in _lines)
            {
                if (line.Data.Length < 2)
                    continue;

                // 为每条线段创建补丁
                for (int i = 0; i < line.Data.Length - 1; i++)
                {
                    // 检查添加新补丁后是否会超出索引限制
                    int currentVertices = patches.Count * patchProcessingCount * verticesPerPatch;
                    if (currentVertices + patchProcessingCount * verticesPerPatch > maxIndexValue)
                    {
                        break;
                    }

                    Vector2 startPoint = new(line.Data[i].X/1000, line.Data[i].Y / 1000);
                    Vector2 endPoint = new(line.Data[i + 1].X / 1000, line.Data[i + 1].Y / 1000);

                    int patchStartIndex = controlPoints.Count;

                    if (UseBezierCurve)
                    {
                        // 使用贝塞尔曲线
                        Vector3 p0 = new(startPoint.X, startPoint.Y, 0);
                        Vector3 p1 = new(startPoint.X + (endPoint.X - startPoint.X) * 0.25f, startPoint.Y + (endPoint.Y - startPoint.Y) * 0.25f, 0);
                        Vector3 p2 = new(startPoint.X + (endPoint.X - startPoint.X) * 0.75f, startPoint.Y + (endPoint.Y - startPoint.Y) * 0.75f, 0);
                        Vector3 p3 = new(endPoint.X, endPoint.Y, 0);

                        // 添加16个控制点
                        for (int j = 0; j < 4; j++)
                        {
                            float t = j / 3.0f;
                            Vector3 point = Bezier(p0, p1, p2, p3, t);
                            controlPoints.Add(point);
                            controlPoints.Add(point + new Vector3(0.01f, 0.01f, 0)); // 轻微偏移以创建宽度
                            controlPoints.Add(point + new Vector3(-0.01f, -0.01f, 0));
                            controlPoints.Add(point);
                        }
                    }
                    else
                    {
                        // 不使用贝塞尔曲线，使用直线
                        Vector3 p0 = new Vector3(startPoint.X, startPoint.Y, 0);
                        Vector3 p1 = new Vector3(endPoint.X, endPoint.Y, 0);

                        // 添加16个控制点（重复添加以满足数量要求）
                        for (int j = 0; j < 4; j++)
                        {
                            controlPoints.Add(p0);
                            controlPoints.Add(p0 + new Vector3(0.01f, 0.01f, 0)); // 轻微偏移以创建宽度
                            controlPoints.Add(p1 + new Vector3(-0.01f, -0.01f, 0));
                            controlPoints.Add(p1);
                        }
                    }

                    // 创建16个索引（对应16个控制点）
                    int[] indices = new int[controlPointsPerPatch];
                    for (int j = 0; j < controlPointsPerPatch; j++)
                    {
                        indices[j] = patchStartIndex + j;
                    }

                    // 添加补丁
                    patches.Add(new GenericLineMesh.GenericLinePatch(false, indices));
                }
            }

            // 设置GenericLineMesh的属性
            _lineMesh.SetLineData(controlPoints.ToArray(), patches.ToArray());

            // 使用线条的平均宽度
            float averageWidth = 2;// _lines.Average(l => l.Width);
            _lineMesh.Size = averageWidth > 0 ? averageWidth : 0.01f;
            _lineMesh.Tessellation = 4; // 降低细分程度以提高性能
        }

        // 贝塞尔曲线计算辅助方法
        private Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
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
        }

        public int ZIndex { get; set; }

        public void Draw(EntityManager entityManager, RenderLayerDescription layerDescription)
        {
            var assetsService = Application.Current.Container.Resolve<AssetsService>();

            // Load the standard effect
            Effect standardEffect = assetsService.Load<Effect>(EvergineContent.Effects.LineEffect);

            layerDescription.Order = ZIndex;
            // Create a material using the custom RenderLayer
            var material = new Material(standardEffect)
            {
                LayerDescription = layerDescription,
            };


            var tectTrans = new Transform3D
            {
                Position = new Vector3(0, 0, 0),
                Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi) // 无需旋转，默认面向 Z+
            };
            // Apply the material to an entity
            Entity primitive = new Entity()
                .AddComponent(tectTrans)
                .AddComponent(new MaterialComponent() { Material = material })
                .AddComponent(_lineMesh)
                .AddComponent(new MeshRenderer());
            entityManager.Add(primitive);
        }
    }
}
