using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Data.Providers;
public class LineDataProvider : VectorDataProvider
{
    private LineData _data;

    public LineDataProvider(LineData data)
    {
        _data = data;
    }


    public override IVectorData GetData() => _data;

    protected override void DoLoadData()
    {
    }

    public override void Dispose()
    {
        _data.Dispose();
    }

    public override DataMessage GetMessage(int index) => null;
}