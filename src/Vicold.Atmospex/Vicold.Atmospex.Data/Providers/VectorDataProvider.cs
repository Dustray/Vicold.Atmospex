using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Data.Providers;
public abstract class VectorDataProvider : IVectorDataProvider
{
    public string Name { get; set; } = "dsds";

    public bool IsLoaded
    {
        get; set;
    }

    public int Count
    {
        get; private set;
    }

    public int Index
    {
        get; set;
    }

    public abstract IVectorData GetData();

    public void LoadData()
    {
        if (IsLoaded)
        {
            return;
        }

        DoLoadData();
        IsLoaded = true;
    }
    protected abstract void DoLoadData();

    public abstract void Dispose();

    public void ReLoadData()
    {

    }

    public abstract DataMessage GetMessage(int index);
}