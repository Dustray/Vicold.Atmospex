using Vicold.Atmospex.Algorithm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Earth.Projection
{
    public class Projection4EqualLonLat : EarthProjection
    {
        private const float _kLon0 = 150;
        private const float _interval = 45;
        private const float _intervalV = 45;

        public Projection4EqualLonLat(ProjectionInfo info) : base(info)
        {
            ID = 1;
            Type = ProjectionType.EqualLonLat;
        }

        public override bool Geo2IndexInternal(double lon, double lat, out double x, out double y)
        {
            x = lon * _interval;
            y = lat * _intervalV;
            return true;
        }

        public override bool Index2GeoInternal(double x, double y, out double lon, out double lat)
        {
            lon = x / _interval;
            lat = y / _intervalV;
            return true;
        }
    }
}
