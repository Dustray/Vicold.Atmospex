using System;
using System.Collections.Generic;

namespace Vicold.Atmospex.Algorithm;

public class LineTileAlgorithm
{
    public static List<ReadOnlyMemory<float>>
        CreateTileLines(ReadOnlyMemory<float> dataArray)
    {
        var span = dataArray.Span;

        var result = new List<ReadOnlyMemory<float>>();

        if (span.Length < 2)
        {
            return result;
        }

        var blockStart = 0; // 当前块起始索引（在原始数组里）
        var currentGridX = GetGridIndex(span[0]);
        var currentGridY = GetGridIndex(span[1]);

        for (var i = 2; i < span.Length; i += 2)
        {
            var x = span[i];
            var y = span[i + 1];


            var gridX = GetGridIndex(x);
            var gridY = GetGridIndex(y);

            if (gridX != currentGridX || gridY != currentGridY)
            {
                var blockLength = i - blockStart + 2; // 包含当前点
                if (blockLength >= 4)
                {
                    result.Add(dataArray.Slice(blockStart, blockLength));
                }

                blockStart = i; // 新块从当前点开始
                currentGridX = gridX;
                currentGridY = gridY;
            }
        }

        // 添加最后一个块
        if (span.Length - blockStart >= 4)
        {
            result.Add(dataArray.Slice(blockStart));
        }

        return result;
    }

    private static int GetGridIndex(float coordinate)
    {
        return (int)(Math.Floor(coordinate / 10)) * 10;
    }
}
