using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.DataService.Provider;

namespace Vicold.Atmospex.Data.Providers;

public interface IGridDataProvider : IProvider
{
    int Width { get; }

    int Height { get; }

    int Level { get; set; }

    double LonInterval { get; }

    double LatInterval { get; }

    double StartLongitude { get; }

    double StartLatitude { get; }

    double EndLongitude { get; }

    double EndLatitude { get; }

    GridData GetData();

    GridData GetData(int index);
}
