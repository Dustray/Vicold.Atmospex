using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace Vicold.Atmospex.Shell.Helpers.Converters;
public class HexToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return new SolidColorBrush(HexToColorConverter.ConvertToColor(value, targetType, parameter, language));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is SolidColorBrush brush)
        {
            return HexToColorConverter.ConvertBackToHex(brush.Color, targetType, parameter, language);
        }

        return "#00000000";
    }
}