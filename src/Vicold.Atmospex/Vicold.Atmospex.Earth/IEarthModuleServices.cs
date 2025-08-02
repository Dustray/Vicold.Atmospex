using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Earth.Events;
using Vicold.Atmospex.Earth.Projection;

namespace Vicold.Atmospex.Earth;
public interface IEarthModuleServices
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
    void Initialize();
}
