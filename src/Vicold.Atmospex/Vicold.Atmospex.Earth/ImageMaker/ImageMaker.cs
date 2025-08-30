using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.DataService.Provider;
using Vicold.Atmospex.Earth.Projection;

namespace Vicold.Atmospex.Earth.ImageMaker;

public class EMImage
{

    public static ImageBound CalculateBound(IGridDataProvider provider, GridData grid, IProjection projection, bool fore_no_prj = false)
    {
        int srid = projection.ID;
        var ps = ((EarthProjection)projection).Info;

        double x_interval = provider.LonInterval;
        double y_interval = provider.LatInterval;

        //var grid = provider.GetGrid(band);

        double west = provider.StartLongitude;
        double north = provider.StartLatitude;
        double east = provider.EndLongitude;
        double south = provider.EndLatitude;

        var validBounds = grid.ValidSubRegion;

        if (south < ps.South)
            south = ps.South;

        if (north > ps.North)
            north = ps.North;

        double lon0 = ps.LonCenter;
        double lat0 = ps.LatCenter;
        double sourceWest = west;
        double sourceEast = east;
        if (east > (lon0 + 180))
        {
            east = lon0 + 180;
            if (west > east - 360)
                west = east - 360;
        }

        double minx = double.MaxValue, miny = double.MaxValue;
        double maxx = double.MinValue, maxy = double.MinValue;

        int y_size = grid.Height;
        int x_size = grid.Width;

        int start_x = 0;
        int start_y = 0;

        if (validBounds != null)
        {
            start_x = validBounds.Value.left;//--------改
            int end_x = validBounds.Value.right;//--------改

            start_y = validBounds.Value.top;//--------改
            int end_y = validBounds.Value.bottom;//--------改

            x_size = (end_x - start_x + 1);
            y_size = (end_y - start_y + 1);
        }

        if (srid != 1001 || fore_no_prj)
        {
            projection.Geo2Index(west, south, out minx, out miny);
            projection.Geo2Index(east, north, out maxx, out maxy);
        }
        else
        {
            //find miny maxy
            double x = 0, y = 0;
            double x_interval_abs = Math.Abs(x_interval);

            for (int i = 0; i < x_size; ++i)
            {
                double lon = west + (i + start_x) * x_interval_abs;

                double[,] updown = new double[,]
                {
                        {lon, north},
                        {lon, south}
                };

                for (int k = 0; k < 2; k++)
                {
                    projection.Geo2Index(updown[k, 0], updown[k, 1], out x, out y);
                    if (y < miny)
                        miny = y;
                    else if (y > maxy)
                        maxy = y;
                    if (x < minx)
                        minx = x;
                    else if (x > maxx)
                        maxx = x;
                }
            }
        }

        if (maxy < miny)
        {
            var temp = miny;
            miny = maxy;
            maxy = temp;
        }

        ImageBound ibounds = new ImageBound()
        {
            West = west,
            East = east,
            North = north,
            South = south,

            LonInterval = x_interval,
            LatInterval = y_interval,

            MinX = minx,
            MinY = miny,
            MaxX = maxx,
            MaxY = maxy,

            XOffset = start_x,
            YOffset = start_y,
            XSize = x_size,
            YSize = y_size
        };

        return ibounds;
    }

    //public static Image RenderImage(IGridDataProvider provider, GridData imagedata, IProjection prj, ImageBound bounds, IPalette palette)
    //{
    //    if (imagedata == null) return null;
    //    double world_height = (bounds.MaxY - bounds.MinY);
    //    double world_width = (bounds.MaxX - bounds.MinX);

    //    int bmp_width = bounds.XSize;
    //    double aspect_ratio = world_height / world_width;
    //    int bmp_height = (int)(aspect_ratio * bmp_width);

    //    double xResolution = world_width / (bmp_width - 1);
    //    double yResolution = world_height / (bmp_height - 1);
    //    var imageBuffer = new Image();
    //    imageBuffer.Create(bmp_width, bmp_height, true, Format.Rgba8);
    //    imageBuffer.Lock();
    //    //GD.Print($"{bmp_height}  {bmp_width}");

    //    int endY = bounds.YOffset + bounds.YSize - 1;

    //    // 为了把右半部分挪到左边
    //    var cutX = bmp_width;
    //    if (bounds.East < provider.EndLongitude)
    //    {
    //        var ra = bounds.East / provider.EndLongitude;
    //        cutX = (int)(ra * bmp_width);
    //    }
    //    //GD.Print(imagedata.NoDataValue);

    //    Parallel.For(0, bmp_height, y =>
    //    //    for (var y = 0; y < bmp_height; y++)
    //    {
    //        double xx = 0;
    //        double yy = bounds.MaxY - y * yResolution;
    //        prj.Index2Geo(xx, yy, out double lon, out double lat);
    //        int row = -1;
    //        if (bounds.LatInterval > 0)
    //        {
    //            row = (int)((lat - bounds.South) / bounds.LatInterval);
    //        }
    //        else
    //        {
    //            row = (int)((lat - bounds.North) / bounds.LatInterval);
    //        }

    //        var yS = bounds.LatInterval < 0 ? bmp_height - y - 1 : y;

    //        if (row >= bounds.YOffset && row <= endY)
    //        {
    //            for (int x = 0; x < bmp_width; x++)
    //            {
    //                var xS = x;
    //                if (cutX != bmp_width)
    //                {// 数据和地图经纬度位置不同时左右互换
    //                    xS = x >= cutX ? x - cutX : x + bmp_width - cutX;
    //                }

    //                int col = x + bounds.XOffset;
    //                var ci = imagedata.ReadFloat(col, row);//--------改
    //                Color color;
    //                if (imagedata.NoDataValue == ci)
    //                {
    //                    color = Colors.Transparent;
    //                }
    //                else
    //                {
    //                    color = palette.SelectColor(ci);
    //                }
    //                //GD.Print($"{x}, {y}, {color}");
    //                imageBuffer.SetPixel(xS, yS, color);
    //            }
    //        }
    //    });//
    //    imageBuffer.Unlock();
    //    //imageBuffer.SavePng($@"d:\222.jpg");

    //    return imageBuffer;
    //}
}