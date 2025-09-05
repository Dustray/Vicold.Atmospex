using Vicold.Atmospex.Earth.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Projection
{
    public abstract class EarthProjection(ProjectionInfo info) : IProjection
    {
        public int ID
        {
            get; set;
        }

        protected Earth2D _bounds = new Earth2D();

        public ProjectionInfo Info
        {
            get; protected set;
        } = info;

        public const double RAD_TO_DEG = 57.29577951308232;

        public const double DEG_TO_RAD = .0174532925199432958;

        protected void _CalculateMapBounds(ProjectionInfo info)
        {
            double left = 0;
            double top = 0;
            double right = 0;
            double bottom = 0;

            string name = info.Name;
            double lon0 = info.LonCenter;
            double lat0 = info.LatCenter;
            double startLon = Math.Ceiling(lon0 - 180);
            double endLon = Math.Floor(lon0 + 180);
            double startLat = info.North;
            double endLat = info.South;

            double x = 0, y = 0;
            switch (name)
            {
                case "兰伯特":
                    //圆心坐标
                    double x0, y0;
                    this.Geo2Index(lon0, 90.0f, out x0, out y0);
                    //下边界用中心经度和最小纬度计算
                    this.Geo2Index(lon0, Info.South, out x, out bottom);
                    //计算半径
                    double dx = x - x0;
                    double dy = bottom - y0;
                    double radius = (float)Math.Sqrt(dx * dx + dy * dy);
                    left = x0 - radius;
                    right = x0 + radius;
                    //上边界
                    this.Geo2Index(startLon, Info.South, out x, out top);

                    break;

                case "北半球极射赤面投影":
                    if (lat0 > 0)
                    {
                        this.Geo2Index(lon0, endLat, out x, out y);
                        bottom = y;
                        top = -bottom;

                        this.Geo2Index(lon0 + 90, endLat, out x, out y);
                        right = x;
                        left = -right;
                    }
                    else
                    {
                        this.Geo2Index(lon0, startLat, out x, out y);
                        bottom = y;
                        top = -bottom;
                        if (bottom > 0)
                        {
                            top = bottom;
                            bottom = -top;
                        }
                        this.Geo2Index(lon0 + 90, startLat, out x, out y);
                        right = x;
                        left = -right;
                    }
                    break;

                case "等经纬":
                case "墨卡托":
                default:
                    this.Geo2Index(startLon, startLat, out left, out top);
                    this.Geo2Index(endLon, endLat, out right, out bottom);
                    break;
            }
            World2D min = new World2D(left, bottom);
            World2D max = new World2D(right, top);
            _bounds.SetExtents(min, max);
        }

        public bool Index2Geo(double x, double y, out double lon, out double lat)
        {
            var res =  Index2GeoInternal(x, y, out lon, out lat);
            lon *= Info.WorldScale;
            lat *= Info.WorldScale;
            return res;
        }
        public bool Geo2Index(double lon, double lat, out double x, out double y)
        {
            var res =Geo2IndexInternal(lon, lat, out x, out y);
            x /= Info.WorldScale;
            y /= Info.WorldScale;
            return res;
        }

        public abstract bool Index2GeoInternal(double x, double y, out double lon, out double lat);

        public abstract bool Geo2IndexInternal(double lon, double lat, out double x, out double y);


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
