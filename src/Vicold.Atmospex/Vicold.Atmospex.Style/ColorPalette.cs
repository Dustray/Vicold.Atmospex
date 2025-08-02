using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Style
{
    public enum RenderType
    {
        Bitmap,
        Contour,
    }

    public class ColorPalette : IPalette
    {
        private ColorItem[] _colors;

        public ColorPalette(int count)
        {
            _colors = new ColorItem[count];
        }
        public ColorPalette(int count, string name) : this(count)
        {
            Name = name;
        }

        public bool AutoValue { get; set; } = true;

        public RenderType RenderType { get; set; } = RenderType.Bitmap;

        public int Count => _colors.Length;

        public float[] ContourAnaValues { get; set; }
        public bool UseAnaValues { get; set; }

        public string Name { get; set; }

        public ColorItem Get(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new Exception("越界");
            }
            return _colors[index];
        }

        public void Set(int index, ColorItem color)
        {
            if (index < 0 || index >= Count)
            {
                throw new Exception("越界");
            }

            _colors[index] = color;
        }

        public ColorItem Select(float value, bool isLessThanValue = true)
        {
            if (isLessThanValue)
            {
                var ret = LowerBound(value);
                if (ret == Count)
                    ret--;
                return _colors[ret];
            }
            else
            {
                var ret = Math.Max(0, UpperBound(value) - 1);
                return _colors[ret];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLessThanValue">返回查询值小于ColorItem.Value的ColorItem</param>
        /// <returns></returns>
        public int SelectIndex(float value, bool isLessThanValue = true)
        {
            if (isLessThanValue)
            {
                var ret = LowerBound(value);
                if (ret == Count)
                    ret--;
                return ret;
            }
            else
            {
                var ret = UpperBound(value);
                return Math.Max(0, ret - 1);
            }
        }

        /// <summary>
        /// Find first index which value greater or equal to value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>first index value is greater or equal to value, if value is the greatest, entry count returns.</returns>
        private int LowerBound(float value)
        {
            int first = 0, last = Count;
            int count = last;
            int it;
            while (count > 0)
            {
                int step = count / 2;
                it = first;
                it += step;

                if (_colors[it].Value < value)
                {
                    first = ++it;
                    count -= step + 1;
                }
                else
                {
                    count = step;
                }
            }
            return first;
        }

        /// <summary>
        /// Find first index which value greater to value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>first index value is greater to value, if value is the greatest, entry count returns.</returns>
        private int UpperBound(float value)
        {
            int first = 0, last = Count;
            int count = last;
            int it;
            while (count > 0)
            {
                int step = count / 2;
                it = first;
                it += step;

                if (_colors[it].Value <= value)
                {
                    first = ++it;
                    count -= step + 1;
                }
                else
                {
                    count = step;
                }
            }
            return first;
        }
    }
}
