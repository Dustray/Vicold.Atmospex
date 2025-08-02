using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Projection
{
    public interface IProjection
    {
        int ID
        {
            get;
        }

        bool Index2Geo(double x, double y, out double lon, out double lat);

        bool Geo2Index(double lon, double lat, out double x, out double y);

    }
}
