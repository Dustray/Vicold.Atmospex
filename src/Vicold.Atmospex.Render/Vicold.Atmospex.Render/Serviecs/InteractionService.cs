using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evergine.Framework.Services;

namespace Vicold.Atmospex.Render.Serviecs;
public class InteractionService : Service
{
    public event EventHandler CameraReset;

    public float Displacement
    {
        get; set;
    }

    public void ResetCamera()
    {
        this.CameraReset?.Invoke(this, EventArgs.Empty);
    }
}