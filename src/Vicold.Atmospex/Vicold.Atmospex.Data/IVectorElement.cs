using Vicold.Atmospex.Data.Vector;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Data
{
    public interface IVectorElement
    {
        /// <summary>
        /// 标号
        /// </summary>
        int Token { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        object Data { get; }

        /// <summary>
        /// 矢量数据类型
        /// </summary>
        VectorType Type { get; }

        /// <summary>
        /// 位置
        /// </summary>
        Float2 Position { get; set; }

        /// <summary>
        /// 偏移
        /// </summary>
        Float2 Offset { get; set; }
    }
}
