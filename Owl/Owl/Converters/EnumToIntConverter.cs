using System;
using System.Globalization;
using System.Net;
using Avalonia.Data.Converters;

namespace Owl.Converters;

public class EnumToIntConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is HttpStatusCode)
        {
            return (int)value;
        }
        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
