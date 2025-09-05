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

        private ICoordinateTransformation _ctf_G2W;
        private ICoordinateTransformation _ctf_W2G;

        public Projection4Mercator(ProjectionInfo info) : base(info)
        {
            var system = new CoordinateSystemFactory();
            var transformation = new CoordinateTransformationFactory();
            //ICoordinateTransformation trans = transformation.CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84, toCS);
            var utm35ETRS = system.CreateFromWkt(
                    "PROJCS[\"ETRS89 / ETRS-TM35\",GEOGCS[\"ETRS89\",DATUM[\"D_ETRS_1989\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",27],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]");
            // 带数=（经度整数位/6）的整数部分+31
            var utm33 = ProjectedCoordinateSystem.WGS84_UTM(49, true);

            _ctf_G2W = transformation.CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84, utm33);
            _ctf_W2G = transformation.CreateFromCoordinateSystems(utm33, GeographicCoordinateSystem.WGS84);
        }

        public override bool Geo2IndexInternal(double lon, double lat, out double x, out double y)
        {
            lon = GeographyAlgorithm.StandardLongitudeConvert(lon, -180, 180);
            var s = _ctf_G2W.MathTransform.Transform(new double[] { lon, lat });
            x = s[0] / 10000;
            y = s[1] / 10000;
            return true;
        }

        public override bool Index2GeoInternal(double x, double y, out double lon, out double lat)
        {
            var s = _ctf_G2W.MathTransform.Transform(new double[] { x * 10000, y * 10000 });
            lon = s[0];
            lat = s[1];
            return true;
        }
    }
}
