using Evergine.Common.Attributes;
using Evergine.Components.Graphics3D;
using Evergine.Components.Primitives;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Render.Frame.Meshes
{
    public class GenericLineMesh : PrimitiveBaseMesh
    {
        private class GenericLinePatch(bool mirrorZ, int[] indices)
        {
            public readonly int[] Indices = indices;
            public readonly bool MirrorZ = mirrorZ;
        }

        private Vector3[]? _controlPoints;
        private GenericLinePatch[]? _patches;
        private float size;
        private int tessellation;

        [RenderPropertyAsFInput(1.1920929E-07f, float.MaxValue)]
        public float Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value <= 0f)
                {
                    throw new ArgumentOutOfRangeException("Size");
                }

                size = value;
                NotifyPropertyChange();
            }
        }

        [RenderPropertyAsInput(3, 25, AsSlider = true, DesiredChange = 1, DesiredLargeChange = 2, DefaultValue = 16)]
        public int Tessellation
        {
            get
            {
                return tessellation;
            }
            set
            {
                if (value < 3)
                {
                    throw new ArgumentOutOfRangeException("Tessellation");
                }

                tessellation = value;
                NotifyPropertyChange();
            }
        }

        public GenericLineMesh()
        {
            size = 1f;
            tessellation = 16;
        }

        public void AddLine()
        {

        }

        protected override int GetPrimitiveHashCode()
        {
            return ((76928 * 395) + size.GetHashCode()) * -1521134295 + tessellation.GetHashCode();
        }

        protected override void Build(PrimitiveModelBuilder builder)
        {
            if (_patches == null)
            {
                return;
            }

            int num = tessellation * tessellation * 6 * 2;
            int num2 = tessellation * tessellation * 6 * 4;
            int num3 = (tessellation + 1) * (tessellation + 1) * 2;
            int num4 = (tessellation + 1) * (tessellation + 1) * 4;
            int num5 = 6;
            int num6 = 4;
            builder.SetCapacity(num * num6 + num2 * num5, num3 * num6 + num4 * num5);
            GenericLinePatch[] array = _patches;
            foreach (GenericLinePatch teapotPatch in array)
            {
                TessellatePatch(teapotPatch, tessellation, new Vector3(size, size, size), builder);
                TessellatePatch(teapotPatch, tessellation, new Vector3(0f - size, size, size), builder);
                if (teapotPatch.MirrorZ)
                {
                    TessellatePatch(teapotPatch, tessellation, new Vector3(size, size, 0f - size), builder);
                    TessellatePatch(teapotPatch, tessellation, new Vector3(0f - size, size, 0f - size), builder);
                }
            }
        }

        private void TessellatePatch(GenericLinePatch patch, int tessellation, Vector3 scale, PrimitiveModelBuilder builder)
        {
            if (_controlPoints == null)
            {
                return;
            }

            Vector3[] array = new Vector3[16];
            for (int i = 0; i < 16; i++)
            {
                int num = patch.Indices[i];
                array[i] = _controlPoints[num] * scale;
            }

            bool isMirrored = Math.Sign(scale.X) != Math.Sign(scale.Z);
            CreatePatchIndices(tessellation, isMirrored, builder);
            CreatePatchVertices(array, tessellation, isMirrored, builder);
        }

        private void CreatePatchIndices(int tessellation, bool isMirrored, PrimitiveModelBuilder builder)
        {
            int num = tessellation + 1;
            int[] array = new int[6];
            for (int i = 0; i < tessellation; i++)
            {
                for (int j = 0; j < tessellation; j++)
                {
                    array[0] = i * num + j;
                    array[1] = (i + 1) * num + j;
                    array[2] = (i + 1) * num + j + 1;
                    array[3] = i * num + j;
                    array[4] = (i + 1) * num + j + 1;
                    array[5] = i * num + j + 1;
                    if (isMirrored)
                    {
                        Array.Reverse(array);
                    }

                    int[] array2 = array;
                    foreach (int num2 in array2)
                    {
                        builder.AddIndex(builder.VerticesCount + num2);
                    }
                }
            }
        }

        private void CreatePatchVertices(Vector3[] patch, int tessellation, bool isMirrored, PrimitiveModelBuilder builder)
        {
            for (int i = 0; i <= tessellation; i++)
            {
                float num = (float)i / (float)tessellation;
                float x = num;
                for (int j = 0; j <= tessellation; j++)
                {
                    float num2 = (float)j / (float)tessellation;
                    float num3 = num2;
                    Vector3 p = Bezier(patch[0], patch[1], patch[2], patch[3], num);
                    Vector3 p2 = Bezier(patch[4], patch[5], patch[6], patch[7], num);
                    Vector3 p3 = Bezier(patch[8], patch[9], patch[10], patch[11], num);
                    Vector3 p4 = Bezier(patch[12], patch[13], patch[14], patch[15], num);
                    Vector3 position = Bezier(p, p2, p3, p4, num2);
                    Vector3 p5 = Bezier(patch[0], patch[4], patch[8], patch[12], num2);
                    Vector3 p6 = Bezier(patch[1], patch[5], patch[9], patch[13], num2);
                    Vector3 p7 = Bezier(patch[2], patch[6], patch[10], patch[14], num2);
                    Vector3 p8 = Bezier(patch[3], patch[7], patch[11], patch[15], num2);
                    Vector3 vector = BezierTangent(p, p2, p3, p4, num2);
                    Vector3 vector2 = BezierTangent(p5, p6, p7, p8, num);
                    Vector3 vector3 = Vector3.Cross(vector, vector2);
                    if (!(vector3.Length() > 0.0001f))
                    {
                        vector3 = ((!(position.Y > 0f)) ? Vector3.Down : Vector3.Up);
                    }
                    else
                    {
                        vector3.Normalize();
                        if (isMirrored)
                        {
                            vector3 = -vector3;
                        }
                    }

                    if (isMirrored)
                    {
                        num3 = 0f - num3;
                    }

                    builder.AddVertex(position, vector3, new Vector2(x, num3));
                }
            }
        }

        private float Bezier(float p1, float p2, float p3, float p4, float t)
        {
            return p1 * (1f - t) * (1f - t) * (1f - t) + p2 * 3f * t * (1f - t) * (1f - t) + p3 * 3f * t * t * (1f - t) + p4 * t * t * t;
        }

        private Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = default(Vector3);
            result.X = Bezier(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = Bezier(p1.Z, p2.Z, p3.Z, p4.Z, t);
            return result;
        }

        private float BezierTangent(float p1, float p2, float p3, float p4, float t)
        {
            return p1 * (-1f + 2f * t - t * t) + p2 * (1f - 4f * t + 3f * t * t) + p3 * (2f * t - 3f * t * t) + p4 * (t * t);
        }

        private Vector3 BezierTangent(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = default(Vector3);
            result.X = BezierTangent(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = BezierTangent(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = BezierTangent(p1.Z, p2.Z, p3.Z, p4.Z, t);
            result.Normalize();
            return result;
        }
    }
}
