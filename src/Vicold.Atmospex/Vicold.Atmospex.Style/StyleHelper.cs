using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Configration;

namespace Vicold.Atmospex.Style;


public static class StyleHelper
{
    public static GridStyle? GetPaletteStyle(IConfigModuleService config, string paletteKey)
    {
        var palettePath = Path.Combine(config.PaletteHome, $"{paletteKey}.json");
        if (!File.Exists(palettePath))
        {
            return null;
        }

        var palette = PaletteHelper.GetColorPalette(palettePath);
        if (palette == null)
        {
            return null;
        }

        var style = new GridStyle();
        style.Palette = palette;

        return style;
    }


    public static GridStyle? GetDefaultPaletteStyle(IConfigModuleService config)
    {
        return GetPaletteStyle(config, "default");
    }
}