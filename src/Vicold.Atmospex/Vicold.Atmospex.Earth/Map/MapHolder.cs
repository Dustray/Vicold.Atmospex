using Vicold.Atmospex.Data;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Earth.Projection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Providers;

namespace Vicold.Atmospex.Earth.Map;

public sealed class MapHolder
{
    private readonly GeographyGridLoader _gridLoader;
    private readonly MapFileLoader _mapFileLoader;

    public MapHolder()
    {

        _mapFileLoader = new MapFileLoader();

        _mapFileLoader.LoadData();
        if (EarthModuleService.Current is { } earthService)
        {
            _gridLoader = new GeographyGridLoader(earthService.ProjectionInfo);
            _gridLoader.LoadData();
        }
        else
        {
            throw new Exception("请先初始化EarthModuleService");
        }
    }

    public IVectorDataProvider WorldLineProvider => CreateProvider(_mapFileLoader.WorldLines, "世界边界");


    public IVectorDataProvider WorldPolygonProvider => CreateProvider(_mapFileLoader.WorldPolygon, "世界填充");

    public IVectorDataProvider ChinaCoastalProvider => CreateProvider(_mapFileLoader.ChinaCoastalLines, "中国边界");

    //public LineDataProvider ChinaInlandProvider => new LineDataProvider(_mapFileLoader.ChinaInlandLines);

    public IVectorDataProvider GeoGridProvider => CreateProvider(_gridLoader.GridLines, "经纬线");

    public IVectorDataProvider GeoFontProvider => CreateProvider(_gridLoader.GridFonts, "经纬值");

    private static IVectorDataProvider CreateProvider(IVectorData? worldLines, string name)
    {
        IVectorDataProvider provider;
        if (worldLines is LineData line)
        {
            provider = new LineDataProvider(line);
        }
        else if (worldLines is VectorData font)
        {
            provider = new FontDataProvider(font);
        }
        else
        {
            throw new Exception("不支持的数据类型");
        }

        provider.Name = name;
        return provider;
    }

}
