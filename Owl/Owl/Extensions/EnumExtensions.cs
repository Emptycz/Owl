using System;
using System.Reflection;
using Owl.Attributes;

namespace Owl.Extensions;

public static class EnumExtensions
{
	/// <summary>
	/// Will get the string value for a given enums value, this will
	/// only work if you assign the StringValue attribute to
	/// the items in your enum.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static string? GetDisplayName(this Enum value) {
		// Get the type
		Type type = value.GetType();

		// Get fieldinfo for this type
		FieldInfo? fieldInfo = type.GetField(value.ToString());

		// Get the stringvalue attributes
		DisplayNameAttribute[]? attribs = fieldInfo?.GetCustomAttributes(
			typeof(DisplayNameAttribute), false) as DisplayNameAttribute[];

		// Return the first if there was a match.
		return attribs?.Length > 0 ? attribs[0].StringValue : null;
	}
}
