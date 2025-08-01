using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evergine.Framework.Graphics;
using Evergine.Framework;
using Vicold.Atmospex.Render.Components;
using System.Xml.Linq;

namespace Vicold.Atmospex.Render.Entities;
internal class MyCamera : Entity
{
    public MyCamera(string name)
    {
        Name = name;
    }

    [BindComponent]
    private EverFreeCamera3D freeCamera =new();

    [BindComponent]
    private Camera3D camera3D = new();

    [BindComponent]
    private Transform3D transform = new();


    protected override bool OnAttached()
    {
        //AddComponent(camera3D);
        //AddComponent(transform);
        transform.LocalPosition = new Evergine.Mathematics.Vector3(0, 0, 0);
        transform.LocalRotation = new Evergine.Mathematics.Vector3(-90, 0, 0);

        return base.OnAttached();
    }
}
