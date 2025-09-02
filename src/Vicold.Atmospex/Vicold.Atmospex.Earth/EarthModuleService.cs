using Vicold.Atmospex.Earth.Events;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.GisMap.Map;
using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Earth;
public class EarthModuleService : IEarthModuleService
{
    private static IAppService? _appService;
    public static EarthModuleService? Current
    {
        get; private set;
    }

    public EarthModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
        ProjectionInfo = CreateProjectionInfo(0);
    }


    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }

    public ProjectionInfo ProjectionInfo
    {
        get; private set;
    }

    public IProjection? CurrentProjection
    {
        get; private set;
    }

    public event MapChangedEventHandler? OnMapChanged;

    public event MapChangedEventHandler? OnMapCreated;

    public float Scale
    {
        get; private set;
    }

    public Task InitMap()
    {
        return Task.Run(() =>
        {
            //        var mapHolder = new MapHolder();

            //        var worldLayerLine = new LineLayer(mapHolder.WorldLineProvider, "rmias_world_line");
            //        //var worldLayerPolygon = new LineLayer(mapHolder.WorldPolygonProvider, "rmias_world_polygon");
            //        var chinaCoastalLayer = new LineLayer(mapHolder.ChinaCoastalProvider, "rmias_china_line");
            //        var geoGridLayer = new LineLayer(mapHolder.GeoGridProvider, "rmias_geo_line");
            //        var geoFontLayer = new FontLayer(mapHolder.GeoFontProvider, "rmias_geo_value");

            //        var manager = _globalBus.GetTransport<ILayerManager>();
            //        manager.AddLayer(geoGridLayer);
            //        manager.AddLayer(geoFontLayer);
            //        manager.AddLayer(worldLayerLine);
            //        manager.AddLayer(chinaCoastalLayer);
        });
    }

    public void ChangeScale(float scale)
    {
        Scale = scale;
        OnMapChanged?.Invoke(this, new MapChangedEventArgs(Scale));
    }

    public void MapCreate()
    {
        OnMapCreated?.Invoke(this, new MapChangedEventArgs(Scale));
    }

    public void Initialize()
    {
        Current = this;
        ChangeProjection(ProjectionType.EqualLonLat);
    }


    public void ChangeProjection(ProjectionType projectionType)
    {
        if (projectionType == ProjectionType.Mercator)
        {
            CurrentProjection = new Projection4Mercator(ProjectionInfo);
        }
        else if (projectionType == ProjectionType.EqualLonLat)
        {
            CurrentProjection = new Projection4EqualLonLat(ProjectionInfo);
        }
    }

    private ProjectionInfo CreateProjectionInfo(double lonCenter)
    {
        var west = lonCenter - 180;
        var east = lonCenter + 180;
        var projectionInfo = new ProjectionInfo()
        {
            Name = "DefaultInfo",
            North = 90,
            South = -90,
            West = (float)west,
            East = (float)east,
            MapLonCenter = (float)lonCenter,
        };
        return projectionInfo;
    }
}
