using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Data
{
    public readonly struct GridRegion
    {
        public readonly int left, top, right, bottom;

        public GridRegion(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public bool Contains(in GridRegion volume)
        {
            return (volume.left >= left && volume.top >= top &&
                volume.right <= right && volume.bottom <= bottom);
        }

        public int Width => right - left;

        public int Height => bottom - top;
    }
}
