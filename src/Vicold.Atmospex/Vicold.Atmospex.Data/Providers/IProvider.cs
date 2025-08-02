using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Data.Providers;
public interface IProvider : IDisposable
{
    string Name
    {
        get; set;
    }

    bool IsLoaded
    {
        get; set;
    }

    int Count
    {
        get;
    }

    int Index
    {
        get; set;
    }

    void LoadData();

    void ReLoadData();

    DataMessage GetMessage(int index);
}