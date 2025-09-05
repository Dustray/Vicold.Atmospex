using Vicold.Atmospex.Earth.Events;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Earth.Map;
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
        ChangeProjection(ProjectionType.CloseToReal);
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
        else if (projectionType == ProjectionType.CloseToReal)
        {
            CurrentProjection = new Projection4CloseToReal(ProjectionInfo);
        }
    }

    private static ProjectionInfo CreateProjectionInfo(double lonCenter)
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
