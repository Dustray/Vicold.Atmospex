using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace Vicold.Atmospex.Shell.Helpers.Converters;
public class FloatFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is float floatValue)
        {
            // 格式化为整数部分至少3位，小数部分固定3位
            return floatValue.ToString("000.000");
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}