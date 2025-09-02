using Vicold.Atmospex.Data;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Earth.Projection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Providers;

namespace Vicold.Atmospex.GisMap.Map
{
    public sealed class MapHolder
    {
        private GeographyGridLoader _gridLoader;
        private MapFileLoader _mapFileLoader;

        public MapHolder()
        {

            _mapFileLoader = new MapFileLoader();

            _mapFileLoader.LoadData();
            if (EarthModuleService.Current is { } earthService)
            {
                _gridLoader = new GeographyGridLoader(earthService.ProjectionInfo);
                _gridLoader.LoadData();
            }
        }

        public IVectorDataProvider WorldLineProvider => CreateProvider(_mapFileLoader.WorldLines, "世界边界");


        public IVectorDataProvider WorldPolygonProvider => CreateProvider(_mapFileLoader.WorldPolygon, "世界填充");

        public IVectorDataProvider ChinaCoastalProvider => CreateProvider(_mapFileLoader.ChinaCoastalLines, "中国边界");

        //public LineDataProvider ChinaInlandProvider => new LineDataProvider(_mapFileLoader.ChinaInlandLines);

        public IVectorDataProvider GeoGridProvider => CreateProvider(_gridLoader.GridLines, "经纬线");

        public IVectorDataProvider GeoFontProvider => CreateProvider(_gridLoader.GridFonts, "经纬值");

        private IVectorDataProvider CreateProvider(IVectorData worldLines, string name)
        {
            IVectorDataProvider provider;
            if (worldLines is LineData line)
            {
                provider = new LineDataProvider(line);
            }
            else
            {
                var font = worldLines as VectorData;
                provider = new FontDataProvider(font);
            }

            provider.Name = name;
            return provider;
        }

    }
}
