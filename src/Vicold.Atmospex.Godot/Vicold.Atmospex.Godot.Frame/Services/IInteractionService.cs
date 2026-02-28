using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Vicold.Atmospex.Earth.Events;
using static Godot.XRPositionalTracker;

namespace Vicold.Atmospex.Godot.Frame.Services
{
    public interface IInteractionService
    {
        event MouseMoveEventHandler OnMouseMove;

        OrderClub Order { get; }

        void MouseMove(Vector2 position);
    }
}
