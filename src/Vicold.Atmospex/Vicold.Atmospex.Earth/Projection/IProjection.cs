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

        ProjectionType Type
        {
            get;
        }

        double MinLongitude
        {
            get;
            set;
        }

        double MaxLongitude
        {
            get;
            set;
        }

        double MinLatitude
        {
            get;
            set;
        }

        double MaxLatitude
        {
            get;
            set;
        }

        bool Index2Geo(double x, double y, out double lon, out double lat);

        bool Geo2Index(double lon, double lat, out double x, out double y);

    }
}
