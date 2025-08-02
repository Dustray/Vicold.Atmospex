using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Data.Providers;
public class FontDataProvider : VectorDataProvider
{
    private VectorData _vectorData;

    public FontDataProvider(VectorData vectorData)
    {
        _vectorData = vectorData;
    }

    public override IVectorData GetData() => _vectorData;

    protected override void DoLoadData()
    {
    }

    public override void Dispose()
    {
        _vectorData.Dispose();
    }

    public override DataMessage GetMessage(int index) => null;
}