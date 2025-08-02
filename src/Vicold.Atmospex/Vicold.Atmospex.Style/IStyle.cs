using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Style;
public interface IStyle
{
    IPalette Palette
    {
        get; set;
    }
}
