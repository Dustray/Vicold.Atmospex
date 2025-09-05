using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evergine.Common.Input.Mouse;
using Evergine.Common.Input;
using Evergine.Components.Cameras;
using Evergine.Framework.Graphics;
using Evergine.Framework;
using Vicold.Atmospex.Render.Serviecs;
using Evergine.Mathematics;
using Evergine.Common.Input.Keyboard;

namespace Vicold.Atmospex.Render.Components;
internal class EverFreeCamera3D : FreeCamera3D
{
    [BindComponent(isExactType: false)]
    protected Camera camera;


    // The service will be null when the scene is executed from the Editor.
    [BindService(false)]
    protected MouseInteractionService service;

    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private Quaternion initialOrientation;

    protected override void OnActivated()
    {
        base.OnActivated();
        camera.Transform.Position += new Vector3(1, -1, 0);
        camera.Transform.Rotation = new Vector3(0, 0, 0);
        initialPosition = camera.Transform.Position;
        initialRotation = camera.Transform.Rotation;
        initialOrientation = camera.Transform.Orientation;
        service.SetPosition(initialPosition, initialRotation);
        if (service is { })
        {
            service.CameraPositionChanged = Service_SetCameraPosition;
            service.CameraRotationChanged = Service_SetCameraRotation;
            service.CameraReset += Service_CameraReset;
        }
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        if (service != null)
        {
            service.CameraReset -= Service_CameraReset;
        }
    }

    private void Service_SetCameraPosition(Vector3 position)
    {
        //var position = camera.Transform.Position;
        camera.Transform.Position = position;
    }

    private void Service_SetCameraRotation(Vector3 rotation)
    {
        //var position = camera.Transform.Position;
        camera.Transform.Rotation = rotation;
    }

    private void Service_CameraReset(object sender, ResetCameraEventArgs e)
    {
        if (e.IsResetPosition)
        {
            camera.Transform.Position = initialPosition;
        }

        if (e.IsResetRotation)
        {
            camera.Transform.Rotation = initialRotation;
        }

        camera.Transform.Orientation = initialOrientation;
        service.SetPosition(initialPosition, initialRotation);
    }

    protected override void Update(TimeSpan time)
    {
        var mouseDispatcher = graphicsPresenter.FocusedDisplay?.MouseDispatcher;
        if (mouseDispatcher is { })
        {
            var p = camera.Transform.Position;

            if (mouseDispatcher.ScrollDelta.Y != 0)
            {
                //service.UpdatePosition(p.X, p.Y);
                if (service.ScrollMode == MouseScrollMode.Move)
                {
                    service.ScrollVertical(mouseDispatcher.Position, camera, mouseDispatcher.ScrollDelta.Y);
                }
                else
                {
                    service.ScrollScale(mouseDispatcher.Position, camera, mouseDispatcher.ScrollDelta.Y);
                }
            }

            switch (mouseDispatcher?.ReadButtonState(MouseButtons.Left))
            {
                case ButtonState.Pressing:
                    service.DragMoving(mouseDispatcher.Position, camera, false);
                    break;
                case ButtonState.Pressed:
                    service.DragMoving(mouseDispatcher.Position, camera, true);
                    break;
            }

            switch (mouseDispatcher?.ReadButtonState(MouseButtons.Right))
            {
                case ButtonState.Pressing:
                    service.DragRotating(mouseDispatcher.Position, camera, false);
                    break;
                case ButtonState.Pressed:
                    service.DragRotating(mouseDispatcher.Position, camera, true);
                    break;
            }
        }

        var keyboardDispatcher = graphicsPresenter.FocusedDisplay?.KeyboardDispatcher;
        if (keyboardDispatcher is { })
        {
            if (keyboardDispatcher.IsKeyDown(Keys.Space))
            {
                camera.Transform.Rotation = initialRotation;
                service.SetPosition(null, initialRotation);
            }
            else
            {
                var p = camera.Transform.Position;
                const float keyMoveStep = 0.01f;
                var keyMoveStepAfterScale = keyMoveStep / (1 / (p.Z + 1));
                if (keyboardDispatcher.IsKeyDown(Keys.W) || keyboardDispatcher.IsKeyDown(Keys.Up))
                {
                    p += new Vector3(0, keyMoveStepAfterScale, 0);
                }
                else if (keyboardDispatcher.IsKeyDown(Keys.S) || keyboardDispatcher.IsKeyDown(Keys.Down))
                {
                    p += new Vector3(0, -keyMoveStepAfterScale, 0);
                }

                if (keyboardDispatcher.IsKeyDown(Keys.A) || keyboardDispatcher.IsKeyDown(Keys.Left))
                {
                    p += new Vector3(-keyMoveStepAfterScale, 0, 0);
                }
                else if (keyboardDispatcher.IsKeyDown(Keys.D) || keyboardDispatcher.IsKeyDown(Keys.Right))
                {
                    p += new Vector3(keyMoveStepAfterScale, 0, 0);
                }

                service.UpdatePosition(p.X, p.Y);
            }


            //var pp = keyboardDispatcher?.ReadKeyState(Keys.Space);
            //switch (pp)
            //{
            //    case ButtonState.Pressing:
            //        Service_CameraReset(null, null);
            //        service.ResetCamera(initialPosition, initialRotation);
            //        break;
            //    case ButtonState.Pressed:
            //        Service_CameraReset(null, null);
            //        service.ResetCamera(initialPosition, initialRotation);
            //        break;
            //}
        }
    }

}