

namespace Vicold.Atmospex.Render.Frame.Tools
{
    internal static class ClassConverter
    {
        public static Evergine.Mathematics.Vector3 ToEver(this System.Numerics.Vector3 sys)
        {
            return new Evergine.Mathematics.Vector3(sys.X, sys.Y, sys.Z);
        }
    }
}
