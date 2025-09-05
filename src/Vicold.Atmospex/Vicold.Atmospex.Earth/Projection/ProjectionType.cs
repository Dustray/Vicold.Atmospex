using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Earth.Projection
{
    public enum ProjectionType
    {
        /// <summary>
        /// 墨卡托投影
        /// </summary>
        Mercator,
        /// <summary>
        /// 等经纬度投影
        /// </summary>
        EqualLonLat,
        /// <summary>
        /// 近似真实投影
        /// </summary>
        CloseToReal,
    }
}
