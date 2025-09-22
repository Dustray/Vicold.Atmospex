using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using Vicold.Atmospex.Algorithm;
using System;
using System.Collections.Generic;

namespace Vicold.Atmospex.Earth.Projection
{
    public class Projection4Lambert : EarthProjection
    {
        private readonly ICoordinateTransformation _ctf_G2W;
        private readonly ICoordinateTransformation _ctf_W2G;

        public Projection4Lambert(ProjectionInfo info) : base(info)
        {
            var transformationFactory = new CoordinateTransformationFactory();
            var coordinateSystemFactory = new CoordinateSystemFactory();

            // 创建WGS84地理坐标系
            var ellipsoid = Ellipsoid.WGS84;
            var datum = coordinateSystemFactory.CreateHorizontalDatum("WGS 1984", DatumType.HD_Geocentric, ellipsoid, null);
            var gcs = coordinateSystemFactory.CreateGeographicCoordinateSystem(
                "WGS 1984", 
                AngularUnit.Degrees, 
                datum,
                PrimeMeridian.Greenwich, 
                new AxisInfo("Lon", AxisOrientationEnum.East),
                new AxisInfo("Lat", AxisOrientationEnum.North)
            );

            // 设置中国中心的参数
            // 中国中心约为东经105度，北纬36度
            double chinaCentralLat = Info.LatCenter > 0 ? Info.LatCenter : 36; // 如果未指定或无效，默认使用36°N
            double chinaCentralLon = Info.LonCenter > 0 ? Info.LonCenter : 105; // 如果未指定或无效，默认使用105°E
            
            // 创建兰伯特投影参数 - 以中国为中心
            var parameters = new List<ProjectionParameter>(6)
            {
                new ProjectionParameter("latitude_of_origin", chinaCentralLat), // 中国中心纬度
                new ProjectionParameter("central_meridian", chinaCentralLon),  // 中国中心经度
                new ProjectionParameter("standard_parallel_1", 25), // 第一标准纬线 - 适合中国南部
                new ProjectionParameter("standard_parallel_2", 47), // 第二标准纬线 - 适合中国北部
                new ProjectionParameter("false_easting", 0),
                new ProjectionParameter("false_northing", 0)
            };

            // 创建兰伯特投影
            var projection = coordinateSystemFactory.CreateProjection("Lambert Conic Conformal (2SP)", "lambert_conformal_conic_2sp", parameters);

            // 创建投影坐标系
            var coordsys = coordinateSystemFactory.CreateProjectedCoordinateSystem(
                "WGS 1984 / Lambert Conic Conformal", 
                gcs, 
                projection, 
                LinearUnit.Metre, 
                new AxisInfo("East", AxisOrientationEnum.East), 
                new AxisInfo("North", AxisOrientationEnum.North)
            );

            // 创建转换器
            _ctf_G2W = transformationFactory.CreateFromCoordinateSystems(gcs, coordsys);
            _ctf_W2G = transformationFactory.CreateFromCoordinateSystems(coordsys, gcs);
        }
        
        /// <summary>
        /// 创建以中国为中心的兰伯特投影实例
        /// </summary>
        /// <returns>以中国为中心的兰伯特投影实例</returns>
        public static Projection4Lambert CreateChinaCentricLambertProjection()
        {
            // 定义中国区域的范围
            // 中国大致范围: 东经73°-135°，北纬4°-53°
            var info = new ProjectionInfo
            {
                Name = "兰伯特", // 设置名称为"兰伯特"，以便EarthProjection基类正确识别
                East = 135,   // 中国最东端经度
                West = 73,    // 中国最西端经度
                North = 53,   // 中国最北端纬度
                South = 4,    // 中国最南端纬度
                WorldScale = 1.0f // 设置适当的世界缩放比例
            };
            
            return new Projection4Lambert(info);
        }

        public override bool Geo2IndexInternal(double lon, double lat, out double x, out double y)
        {
            // 转换经纬度范围以适应兰伯特投影
            if (lon < -180) {
                lon = -180;
            }
            if (lon > 180) {
                lon = 180;
            }
            if (lat < -80) {
                lat = -80;
            }
            if (lat > 80) {
                lat = 80;
            }

            // 使用projnet进行坐标转换
            var s = _ctf_G2W.MathTransform.Transform([lon, lat]);
            // 兰伯特投影的坐标值通常很大，增加一个更大的缩放因子来减小最终的坐标值
            // 测试代码中显示坐标值可达数百万米，所以使用更大的缩放因子
            x = s[0] / 10000; // 使用100000作为缩放因子
            y = s[1] / 10000;
            return true;
        }

        public override bool Index2GeoInternal(double x, double y, out double lon, out double lat)
        {
            // 反向转换，使用相同的缩放因子
            var s = _ctf_W2G.MathTransform.Transform([x * 10000, y * 10000]);
            lon = s[0];
            lat = s[1];
            return true;
        }
    }
}