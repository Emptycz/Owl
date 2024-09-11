using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Owl.Extensions;

namespace Owl.Converters;

public class EnumDisplayNameConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is not Enum enumValue ? null : enumValue.GetDisplayName();
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
