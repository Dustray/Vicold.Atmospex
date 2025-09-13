using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
using Windows.UI;

namespace Vicold.Atmospex.Shell.Helpers.Converters;
public class HexToColorConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return ConvertToColor(value, targetType, parameter, language);
    }

    public static Color ConvertToColor(object value, Type targetType, object parameter, string language)
    {
        try
        {
            var (a, r, g, b) = ParseHex(value?.ToString());
            return ColorHelper.FromArgb(a, r, g, b);
        }
        catch
        {
            return Colors.Transparent; // 解析失败返回透明
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return ConvertBackToHex(value, targetType, parameter, language);
    }

    public static string ConvertBackToHex(object value, Type targetType, object parameter, string language)
    {
        if (value is Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        return "#00000000";
    }

    private static (byte a, byte r, byte g, byte b) ParseHex(string? hex)
    {
        hex = hex?.TrimStart('#') ?? "00000000";

        // 处理短格式（如 #RGB 扩展为 #FFRRGGBB）
        hex = hex.Length switch
        {
            3 => $"FF{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}",
            4 => $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}",
            6 => $"FF{hex}",
            8 => $"{hex}",
            _ => "00000000"
        };
        //if (hex.Length == 3) hex = $"FF{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
        //if (hex.Length == 4) hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";
        //if (hex.Length == 6) hex = $"FF{hex}";
        //if (hex.Length == 8) hex = $"{hex}";

        return
        (
            byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
        );
    }
}