using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Render.ElementEntites;
using Evergine.Common.Graphics;
using Evergine.Common.Graphics.VertexFormats;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using Evergine.Framework.Services;
using Evergine.Mathematics;

namespace Vicold.Atmospex.Render.Components;
public class EverBatchLine : Drawable3D
{
    [BindService]
    private AssetsService assetsService = null;

    private GraphicsContext graphicsContext;

    private LineBatch3D lineBatch;

    public EverBatchLine(List<ElementLine> lines)
    {
        Lines = lines;
    }

    public List<ElementLine> Lines
    {
        get; set;
    }

    public float OffsetY
    {
        get; set;
    } = 0f;

    public int RarefyLevel
    {
        get; set;
    } = 1;

    protected override bool OnAttached()
    {
        this.graphicsContext = Application.Current.Container.Resolve<GraphicsContext>();
        var layer = this.assetsService.Load<RenderLayerDescription>(DefaultResourcesIDs.OpaqueRenderLayerID);

        // Create custom line batch 3D
        this.lineBatch = new LineBatch3D(this.graphicsContext, layer);

        // Add line batch to render
        this.Managers.RenderManager.AddRenderObject(this.lineBatch);

        return base.OnAttached();
    }

    protected override void OnActivated()
    {
        // Enable line batch when the component is activated
        this.lineBatch.IsEnabled = true;
        base.OnActivated();
    }

    protected override void OnDeactivated()
    {
        // Disable line batch when the component is deactivated
        this.lineBatch.IsEnabled = false;
        base.OnDeactivated();
    }

    protected override void OnDetach()
    {
        // Remove line batch from render when the component is detached
        this.Managers.RenderManager.RemoveRenderObject(this.lineBatch);
        base.OnDetach();
    }

    public override void Draw(DrawContext drawContext)
    {
        drawContext.ForceBlitAlphaBlending = true;
        // Draw a sample blue cone
        this.lineBatch.DrawCone(0.5f, 1.0f, Vector3.UnitY, Vector3.Down, Color.Blue);
        lineBatch.DrawLine(Vector3.Zero, Vector3.UnitY * 2.0f, Color.Red);
        // Draw lines from the Lines property
        foreach (var line in Lines)
        {
            if (line.IsVisibile)
            {
                lineBatch.DrawLine(line.Begin + new Vector3(0, OffsetY, 0), line.End + new Vector3(0, OffsetY, 0), line.Color);
            }
        }
    }
}