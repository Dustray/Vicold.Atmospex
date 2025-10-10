using System.Numerics;
using LibTessDotNet;

namespace Vicold.Atmospex.Algorithm;
public class TessResult
{
    public Vector3[] Positions; // 使用 System.Numerics.Vector3
    public ushort[] Indices;    // 如果顶点数会超过 65535，你需要改为 uint 并相应更改 index buffer
}

public static class PolygonTessellatorAlgorithm
{
    // 单环（无洞）三角化
    public static TessResult TessellateSimple(Vector2[] contour)
    {
        if (contour == null || contour.Length < 3)
            return new TessResult { Positions = Array.Empty<Vector3>(), Indices = Array.Empty<ushort>() };

        var tess = new Tess();

        // 把轮廓加进去
        var cv = new ContourVertex[contour.Length];
        for (int i = 0; i < contour.Length; i++)
        {
            cv[i].Position = new Vec3(contour[i].X, contour[i].Y, 0.0f);
        }
        tess.AddContour(cv, ContourOrientation.Original);

        // 进行三角化：要求输出三角形
        tess.Tessellate(WindingRule.Positive, ElementType.Polygons, 3);

        // 生成 positions（来自 tess.Vertices）
        var tverts = tess.Vertices;
        var positions = new Vector3[tverts.Length];
        for (int i = 0; i < tverts.Length; i++)
        {
            var p = tverts[i].Position;
            positions[i] = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }

        // 使用 tess.ElementCount 来确定有效三角形数量（每个三角形 3 个索引）
        int triCount = tess.ElementCount;
        var indices = new ushort[triCount * 3];
        for (int i = 0; i < triCount; i++)
        {
            int baseIdx = i * 3;
            indices[baseIdx + 0] = (ushort)tess.Elements[baseIdx + 0];
            indices[baseIdx + 1] = (ushort)tess.Elements[baseIdx + 1];
            indices[baseIdx + 2] = (ushort)tess.Elements[baseIdx + 2];
        }

        return new TessResult { Positions = positions, Indices = indices };
    }

}
