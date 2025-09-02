using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Earth.Events;
using Vicold.Atmospex.Earth.Projection;

namespace Vicold.Atmospex.Earth;
public interface IEarthModuleService : IModuleService
{
    ProjectionInfo ProjectionInfo
    {
        get;
    }

    IProjection? CurrentProjection
    {
        get;
    }

    void ChangeProjection(ProjectionType projectionType);
    float Scale
    {
        get;
    }
    Task InitMap();

    event MapChangedEventHandler OnMapChanged;

    event MapChangedEventHandler OnMapCreated;

    void ChangeScale(float scale);
}
