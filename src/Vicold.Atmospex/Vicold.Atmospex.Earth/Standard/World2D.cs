using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Standard
{
    public struct World2D
    {
        public World2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public readonly static World2D Zero = new World2D(0, 0);

        public double X { get; set; }

        public double Y { get; set; }

        public Geography2D ToGeography2D()
        {
            return new Geography2D();
        }

        public Screen2D ToScreen2D()
        {
            return new Screen2D();
        }

        public void Ceil(World2D other)
        {
            if (X < other.X)
                X = other.X;
            if (Y < other.Y)
                Y = other.Y;
        }

        public void Floor(World2D other)
        {
            if (X > other.X)
                X = other.X;
            if (Y > other.Y)
                Y = other.Y;
        }
    }
}
