using Owl.Attributes;
using Owl.Services.VariableResolvers;

namespace Owl.Models.Variables;

[RegisterToVariableResolver(typeof(StaticVariableResolver))]
public class StaticVariable : VariableBase
{
	public string Value { get; set; } = string.Empty;
}
