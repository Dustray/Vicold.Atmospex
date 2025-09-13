using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using Vicold.Atmospex.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Projection
{
    public class Projection4Mercator : EarthProjection
    {

        private readonly ICoordinateTransformation _ctf_G2W;
        private readonly ICoordinateTransformation _ctf_W2G;

        public Projection4Mercator(ProjectionInfo info) : base(info)
        {
            var transformation = new CoordinateTransformationFactory();

            // 使用WGS84地理坐标系作为源（经度范围-180到180）
            var geographicCs = GeographicCoordinateSystem.WGS84;

            // 使用Web Mercator作为目标投影
            var webMercator = ProjectedCoordinateSystem.WebMercator;

            // 创建转换器
            _ctf_G2W = transformation.CreateFromCoordinateSystems(geographicCs, webMercator);
            _ctf_W2G = transformation.CreateFromCoordinateSystems(webMercator, geographicCs);
        }

        public override bool Geo2IndexInternal(double lon, double lat, out double x, out double y)
        {
            if (lon < -180)
            {
                lon = -180;
            }

            if (lon > 180)
            {
                lon = 180;
            }

            if (lat < -85.06)
            {
                lat = -85.06;
            }

            if (lat > 85.06)
            {
                lat = 85.06;
            }

            var s = _ctf_G2W.MathTransform.Transform([lon, lat]);
            x = s[0] / 5000;
            y = s[1] / 5000;
            return true;
        }

        public override bool Index2GeoInternal(double x, double y, out double lon, out double lat)
        {
            var s = _ctf_W2G.MathTransform.Transform([x * 5000, y * 5000]);
            lon = s[0];
            lat = s[1];
            return true;
        }
    }
}
