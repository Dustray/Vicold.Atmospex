using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Data.Vector
{
    public struct Float2
    {
        public Float2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public float X { get; set; }
        public float Y { get; set; }


        public static Float2 operator +(Float2 f1, Float2 f2)
        {
            return new Float2(f1.X + f2.X, f1.Y + f2.Y);
        }
    }
}
