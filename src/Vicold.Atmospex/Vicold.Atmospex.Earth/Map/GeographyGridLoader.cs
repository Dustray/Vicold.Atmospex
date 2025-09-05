using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Earth.Projection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Vicold.Atmospex.Earth.Map;

public class GeographyGridLoader
{
    private readonly Color _gridColor = Color.FromArgb(180, 140, 150, 180);
    private readonly Color _gridFontColor = Color.FromArgb(120, 130, 160);
    private ProjectionInfo _projectionInfo;
    public GeographyGridLoader(ProjectionInfo projectionInfo)
    {
        _projectionInfo = projectionInfo;
        GridLines = new LineData();
        GridFonts = new VectorData();
    }

    public void LoadData()
    {
        var latInterval = 2.5f;
        var lonInterval = 10;
        var south = _projectionInfo.South;
        var north = _projectionInfo.North;
        var west = _projectionInfo.West;
        var east = _projectionInfo.East;

        // 画纬线，从南到北
        var i = 0;
        for (var s = south; s <= north; s += lonInterval, i = 0)
        {
            //var data = new float[145 * 2];
            // 从东到西
            //for (var w = west; w <= east; w += latInterval)
            //{
            //    data[i++] = (float)w;
            //    data[i++] = (float)s;
            //}

            var data = new float[2 * 2];
            data[i++] = (float)west;
            data[i++] = (float)s;
            data[i++] = (float)east;
            data[i++] = (float)s;
            GridLines.Add(data, 1, 0, _gridColor);
        }

        // 画经线，从东到西
        i = 0;
        for (var w = west; w <= east; w += lonInterval, i = 0)
        {
            var data = new float[4];
            // 从南到北
            data[i++] = (float)w;
            data[i++] = (float)south;
            data[i++] = (float)w;
            data[i++] = (float)north;

            GridLines.Add(data, 1, 0, _gridColor);
        }


        //画字符
        for (var w = west; w <= east; w += lonInterval)
        {
            GridFonts.Collection.Add(w.ToString(), 14, _gridFontColor, new Float2((float)w, (float)north), new Float2(0, 0), new Float2(0.5f, 1));// 上
            GridFonts.Collection.Add(w.ToString(), 14, _gridFontColor, new Float2((float)w, (float)south), new Float2(0, 0), new Float2(0.5f, 0));// 下
        }

        for (var s = south; s <= north; s += lonInterval)
        {
            GridFonts.Collection.Add(s.ToString(), 14, _gridFontColor, new Float2((float)west, (float)s), new Float2(0, 0), new Float2(1, 0.5f));// 左
            GridFonts.Collection.Add(s.ToString(), 14, _gridFontColor, new Float2((float)east, (float)s), new Float2(0, 0), new Float2(0, 0.5f));// 右
        }
    }

    public LineData GridLines
    {
        get; private set;
    }

    public VectorData GridFonts
    {
        get; private set;
    }
}
