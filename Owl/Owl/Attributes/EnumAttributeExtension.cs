using System;
using System.Reflection;

namespace Owl.Attributes;

/// <summary>
/// This attribute is used to represent a string value
/// for a value in an enum.
/// </summary>
public class ValueAttribute : Attribute {

	#region Properties

	/// <summary>
	/// Holds the stringvalue for a value in an enum.
	/// </summary>
	public string StringValue { get; protected set; }

	#endregion

	#region Constructor

	/// <summary>
	/// Constructor used to init a StringValue Attribute
	/// </summary>
	/// <param name="value"></param>
	public ValueAttribute(string value) {
		this.StringValue = value;
	}

	#endregion

}
