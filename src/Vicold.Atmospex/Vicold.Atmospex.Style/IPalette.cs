using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Style
{
    public interface IPalette     //: IEnumerable<ColorItem>, IEnumerable,
    {
        string Name { get; set; }

        int Count { get; }
        
        bool AutoValue { get; set; }

        RenderType RenderType { get; set; }

        float[] ContourAnaValues { get; set; }
          bool UseAnaValues { get; set; }

        ColorItem Get(int index);

        void Set(int index, ColorItem color);

        ColorItem Select(float value, bool isLessThanValue = true);

        int SelectIndex(float value, bool isLessThanValue = true);
    }
}
