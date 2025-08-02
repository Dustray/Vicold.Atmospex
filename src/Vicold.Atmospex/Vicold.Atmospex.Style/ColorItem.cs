using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Style
{
    public struct ColorItem : IEquatable<ColorItem>/*, IComparable<ColorItem>*/
    {

        public ColorItem(float value, Color color)
            : this(value, color.R, color.G, color.B, color.A)
        { }

        public ColorItem(float value, byte r, byte g, byte b)
            : this(value, r, g, b, 255)
        { }

        public ColorItem(float value, byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
            Value = value;
        }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public float Value { get; set; }

        //public int CompareTo(ColorItem other)
        //{
        //}

        public bool Equals(ColorItem other)
        {
            return Value == other.Value;
        }


        public override string ToString()
        {
            return $"RGBA: {R}, {G}, {B}, {A}";
        }
    }
}
