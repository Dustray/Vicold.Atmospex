using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Algorithm
{
    public static class InterpolationAlgorithm
    {
        /// <summary>
        /// 线性插值算法
        /// </summary>
        public static float Excute(float firstValue, float lastValue, float firstPosition, float lastPosition, float targetPosition)
        {
            // 已假设 targetPosition 在 firstPosition 和 lastPosition 之间
            var proportion = (targetPosition - firstPosition) / (lastPosition - firstPosition);
            return (lastValue - firstValue) * proportion + firstValue;
        }

        //  2     4         8
        //  4    5.2        8

        //  8     6         2
        //  4    5.2        8
    }
}
