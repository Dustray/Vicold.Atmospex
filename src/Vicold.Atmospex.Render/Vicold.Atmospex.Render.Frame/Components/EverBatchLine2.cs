using System;
using System.Collections.Generic;
using Evergine.Common.Graphics;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using Vicold.Atmospex.Data.Vector;

namespace Vicold.Atmospex.Render.Components;
public class EverBatchLine2 : Drawable3D
{
    [BindService]
    private AssetsService assetsService = null;

    private GraphicsContext? graphicsContext;
    private LineBatch3D? lineBatch;
    private readonly RenderLayerDescription? renderLayer;

    public VectorLine[] Lines { get; set; } = [];
    public bool UseBezierCurve { get; set; } = false;
    public float LineThickness { get; set; } = 0.01f;

    public EverBatchLine2(RenderLayerDescription layer)
    {
        renderLayer = layer;
    }

    protected override bool OnAttached()
    {
        graphicsContext = Application.Current.Container.Resolve<GraphicsContext>();

        // Create custom line batch 3D
        lineBatch = new LineBatch3D(graphicsContext, renderLayer);

        // Add line batch to render
        Managers.RenderManager.AddRenderObject(lineBatch);

        return base.OnAttached();
    }

    protected override void OnActivated()
    {
        // Enable line batch when the component is activated
        if (lineBatch is { })
        {
            lineBatch.IsEnabled = true;
        }

        base.OnActivated();
    }

    protected override void OnDeactivated()
    {
        // Disable line batch when the component is deactivated
        if (lineBatch is { })
        {
            lineBatch.IsEnabled = false;
        }

        base.OnDeactivated();
    }

    protected override void OnDetach()
    {
        // Remove line batch from render when the component is detached
        Managers.RenderManager.RemoveRenderObject(lineBatch);
        base.OnDetach();
    }

    public void UpdateLines()
    {
        if (lineBatch == null || Lines.Length == 0)
            return;

        // Clear previous lines
        foreach (var line in Lines)
        {
            if (line.Data.Length < 2)
                continue;

            // Line color
            var c = new Vector4(line.LineColor.R, line.LineColor.G, line.LineColor.B, line.LineColor.A);
            Color lineColor = Color.FromVector4(ref c);
            for (int i = 0; i < line.Data.Length - 1; i++)
            {
                Vector2 startPoint = new(line.Data[i].X, line.Data[i].Y);
                Vector2 endPoint = new(line.Data[i + 1].X, line.Data[i + 1].Y);

                if (UseBezierCurve)
                {
                    // Use Bezier curve
                    Vector3 p0 = new(startPoint.X, startPoint.Y, 0);
                    Vector3 p1 = new(startPoint.X + (endPoint.X - startPoint.X) * 0.25f, startPoint.Y + (endPoint.Y - startPoint.Y) * 0.25f, 0);
                    Vector3 p2 = new(startPoint.X + (endPoint.X - startPoint.X) * 0.75f, startPoint.Y + (endPoint.Y - startPoint.Y) * 0.75f, 0);
                    Vector3 p3 = new(endPoint.X, endPoint.Y, 0);

                    // Add points on Bezier curve and connect them
                    Vector3 previousPoint = Bezier(p0, p1, p2, p3, 0);
                    for (int j = 1; j <= 10; j++)
                    {
                        float t = j / 10.0f;
                        Vector3 currentPoint = Bezier(p0, p1, p2, p3, t);
                        lineBatch.DrawLine(previousPoint, currentPoint, lineColor);
                        previousPoint = currentPoint;
                    }
                }
                else
                {
                    // Draw line directly
                    lineBatch.DrawLine(
                        new Vector3(startPoint.X, startPoint.Y, 0),
                        new Vector3(endPoint.X, endPoint.Y, 0),
                        lineColor);
                }
            }
        }
    }

    public override void Draw(DrawContext drawContext)
    {
        drawContext.ForceBlitAlphaBlending = true;
        // Lines are already drawn in UpdateLines
        UpdateLines();
    }

    // Bezier curve calculation helper method
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
}