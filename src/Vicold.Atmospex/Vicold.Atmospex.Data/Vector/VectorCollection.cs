using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Vicold.Atmospex.Data.Vector
{
    internal class VectorCollection : IVectorCollection
    {
        private IList<IVectorElement> _vectorElements;

        public VectorCollection()
        {
            _vectorElements = new List<IVectorElement>();
        }

        public int Count => _vectorElements.Count;

        public IVectorElement this[int index]
        {
            get { return _vectorElements[index]; }
        }

        public void Add(string font, float fontSize, Color fontColor, Float2 position, Float2 offset, Float2 pivot, string name = null, int token = 0)
        {
            _vectorElements.Add(new FontElement()
            {
                Data = font,
                Name = name,
                Token = token,
                Position = position,
                Offset = offset,
                Pivot = pivot,
                Type = VectorType.Font,
                FontSize = fontSize,
                FontColor = fontColor,
            });
        }

        public void Dispose()
        {
            _vectorElements.Clear();
        }
    }
}
