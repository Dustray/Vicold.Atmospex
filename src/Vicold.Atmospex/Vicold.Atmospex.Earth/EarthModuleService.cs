using Vicold.Atmospex.Earth.Events;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Earth.Map;
using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Earth;
public class EarthModuleService : IEarthModuleService
{
    private static IAppService? _appService;
    private ProjectionType _projectionType;
    public static EarthModuleService? Current
    {
        get; private set;
    }

    public EarthModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
        _projectionType = ProjectionType.CloseToReal;
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

    public event InteractionChangedEventHandler? OnMouseMoved;

    public float Scale
    {
        get; private set;
    }

    public float WorldScale
    {
        get; set;
    } = 1;

    public ProjectionType Projection => _projectionType;

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
        ProjectionInfo = CreateProjectionInfo(0);
        ChangeProjection(_projectionType);
    }


    public void ChangeProjection(ProjectionType projectionType)
    {
        if (CurrentProjection is { } && projectionType == _projectionType)
        {
            return;
        }

        _projectionType = projectionType;
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
        else if (projectionType == ProjectionType.Lambert)
        {
            CurrentProjection = new Projection4Lambert(ProjectionInfo);
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
            WorldScale = WorldScale,
        };
        return projectionInfo;
    }

    public void ChangeMouse(float worldX, float worldY, float screenX, float screenY)
    {
        if (OnMouseMoved is { } && CurrentProjection is { })
        {
            _ = CurrentProjection.Index2Geo(worldX, worldY, out var lon, out var lat);
            OnMouseMoved(this, new InteractionChangedEventArgs()
            {
                WorldCoordinate = new(worldX, worldY),
                ScreenCoordinate = new(screenX, screenY),
                GeoCoordinate = new((float)lon, (float)lat)
            });
        }
    }
}
