using System.Numerics;

namespace Vicold.Atmospex.Earth.Events;

public delegate void MouseMoveEventHandler(object sender, MouseMoveEventArgs e);
public class MouseMoveEventArgs(Vector2 screenPosition, Vector2 worldPosition) : EventArgs
{
    public Vector2 ScreenPosition
    {
        get; set;
    } = screenPosition;
    public Vector2 WorldPosition
    {
        get; set;
    } = worldPosition;
}