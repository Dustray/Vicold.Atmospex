using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Standard
{
    public struct Screen2D
    {
        public Screen2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly static Screen2D Zero = new Screen2D(0, 0);

        public int X { get; set; }

        public int Y { get; set; }

        public World2D ToWorld2D()
        {
            return new World2D();
        }

        public Geography2D ToGeography2D()
        {
            return new Geography2D();
        }

        public void Ceil(Screen2D other)
        {
            if (X < other.X)
                X = other.X;
            if (Y < other.Y)
                Y = other.Y;
        }

        public void Floor(Screen2D other)
        {
            if (X > other.X)
                X = other.X;
            if (Y > other.Y)
                Y = other.Y;
        }
    }
}
