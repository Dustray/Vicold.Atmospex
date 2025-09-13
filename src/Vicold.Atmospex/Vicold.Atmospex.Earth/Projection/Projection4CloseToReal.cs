using Vicold.Atmospex.Algorithm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Earth.Projection
{
    public class Projection4CloseToReal : EarthProjection
    {
        private const float _kLon0 = 150;
        private const float _interval = 45;
        private const float _intervalV = 250;

        public Projection4CloseToReal(ProjectionInfo info) : base(info)
        {
            ID = 1;
        }

        public override bool Geo2IndexInternal(double lon, double lat, out double x, out double y)
        {
            lon = GeographyAlgorithm.StandardLongitudeConvert(lon, -180, 180);

            x = lon * _interval;
            y = lat * _interval * (1 + Math.Abs(lat) / _intervalV);
            return true;
        }

        public override bool Index2GeoInternal(double x, double y, out double lon, out double lat)
        {
            lon = x / _interval;

            if (y > 0)
            {// 北纬
                var bfang = _interval * _interval + (4 * y * _interval) / _intervalV;
                lat = (-_interval + Math.Sqrt(bfang)) / (2 * _interval / _intervalV);
            }
            else
            {// 南纬
                var bfang = _interval * _interval - (4 * y * _interval) / _intervalV; // b方-4ac
                lat = (_interval - Math.Sqrt(bfang)) / (2 * _interval / _intervalV);
            }
            //lat = -lat;
            return true;
        }
    }
}
