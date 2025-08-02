using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Vicold.Atmospex.Data.Vector
{
    public interface IVectorCollection : IDisposable
    {
        int Count { get; }

        IVectorElement this[int index] { get; }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontColor"></param>
        /// <param name="position"></param>
        /// <param name="offset"></param>
        /// <param name="pivot"></param>
        /// <param name="name"></param>
        /// <param name="token"></param>
        void Add(string font, float fontSize, Color fontColor, Float2 position, Float2 offset, Float2 pivot, string name = null, int token = 0);
    }
}
