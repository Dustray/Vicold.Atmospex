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


    float WorldScale
    {
        set; get;
    }

    ProjectionType Projection
    {
        get;
    }

    event MapChangedEventHandler OnMapChanged;

    event MapChangedEventHandler OnMapCreated;

    event InteractionChangedEventHandler OnMouseMoved;

    void ChangeScale(float scale);

    void ChangeMouse(float worldX, float worldY, float screenX, float screenY);
}
