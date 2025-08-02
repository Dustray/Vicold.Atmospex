using HighSmooth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Algorithm;

public static class SmoothAlgorithm
{
    public static unsafe float[] Go(float[] data, int width, int height, float invalidValue)
    {
        float[] vs = new float[data.Length];
        fixed (float* r = &vs[0])
        {
            var source = SourceDataCreator.CreateSourceData(data, width, height, invalidValue);
            Smooth.Excute(source, r, SmoothType.Gauss, SmoothPoint.NinePoint);
        }

        return vs;
    }

    public static unsafe IntPtr Go(IntPtr data, int width, int height, float invalidValue)
    {
        return Go(data, width, height, invalidValue, SmoothType.Gauss);
    }

    public static unsafe IntPtr Go2(IntPtr data, int width, int height, float invalidValue, int smoothType)
    {
        var smt = SmoothType.Gauss;
        switch (smoothType)
        {
            case 1:
                smt = SmoothType.Mean; break;
            case 2:
                smt = SmoothType.Median; break;
            case 3:
                smt = SmoothType.Gauss; break;
        }
        return Go(data, width, height, invalidValue, smt);
    }

    public static unsafe IntPtr Go(IntPtr data, int width, int height, float invalidValue, SmoothType smoothType)
    {
        float[] vs = new float[width * height];
        fixed (float* r = &vs[0])
        {
            var result = new IntPtr(r);
            var source = SourceDataCreator.CreateSourceData(data, width, height, DataType.Float32, invalidValue);
            Smooth.Excute(source, r, smoothType, SmoothPoint.NinePoint);
            return result;
        }

    }
}
