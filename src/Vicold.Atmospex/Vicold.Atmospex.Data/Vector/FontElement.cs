using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Vicold.Atmospex.Data.Vector
{
    public class FontElement: IVectorElement
    {
        /// <summary>
        /// 标号
        /// </summary>
        public int Token { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 矢量数据类型
        /// </summary>
        public VectorType Type { get; internal set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        public Float2 Position { get; set; }

        /// <summary>
        /// 偏移
        /// </summary>
        public Float2 Offset { get; set; }

        /// <summary>
        /// 缩放中轴点 范围0-1
        /// </summary>
        public Float2 Pivot { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public float FontSize { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color FontColor { get; set; }
    }
}
