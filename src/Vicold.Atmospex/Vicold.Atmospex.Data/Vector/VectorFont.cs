using System.Drawing;
using System.Numerics;

namespace Vicold.Atmospex.Data.Vector;

public class VectorFont
    {

        /// <summary>
        /// 数据
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// 像素偏移
        /// </summary>
        public Vector2 Pivot { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public float FontSize { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color FontColor { get; set; }
    }