using Evergine.Framework.Graphics;
using Evergine.Mathematics;

namespace Vicold.Atmospex.Render.Helpers;
public static class VectorExchanger
{
    public static Vector3 ToChangeOverlook(this Vector3 v)
    {
        //return v.ToReverseY().ToChangeYZ();
        return v;// new Vector3(v.X, v.Z, v.Y);
    }

    public static Vector3 ToChangeYZ(this Vector3 v)
    {
        return new Vector3(v.X, v.Z, v.Y);
    }

    public static Vector3 ToReverseY(this Vector3 v)
    {
        return new Vector3(v.X, -v.Y, v.Z);
    }

    public static void ToChangeOverlook(this Transform3D trans)
    {
        ToRotateX(trans, 270);
    }

    public static void ToRotateX(this Transform3D trans, int angle)
    {
        trans.LocalRotation = new Vector3((float)(angle * (System.Math.PI / 180)), 0, 0);
    }

    public static void ToRotateY(this Transform3D trans, int angle)
    {
        trans.LocalRotation = new Vector3(0, (float)(angle * (System.Math.PI / 180)), 0);
    }



    public static void ToReverseY(this Transform3D trans)
    {
        trans.Position = new Vector3(trans.Position.X, trans.Position.Y, -trans.Position.Z);
    }


    public static void ToMoveY(this Transform3D trans,float y)
    {
        trans.Position = new Vector3(trans.Position.X, trans.Position.Y + y, trans.Position.Z);
    }
}
