using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Standard
{
    public struct Geography2D
    {
        public Geography2D(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public readonly static Geography2D Zero = new Geography2D(0, 0);

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public World2D ToWorld2D()
        {
            return new World2D();
        }
        public Screen2D ToScreen2D()
        {
            return new Screen2D();
        }

        public void Ceil(Geography2D other)
        {
            if (Longitude < other.Longitude)
                Longitude = other.Longitude;
            if (Latitude < other.Latitude)
                Latitude = other.Latitude;
        }

        public void Floor(Geography2D other)
        {
            if (Longitude > other.Longitude)
                Longitude = other.Longitude;
            if (Latitude > other.Latitude)
                Latitude = other.Latitude;
        }
    }
}
