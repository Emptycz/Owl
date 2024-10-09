using System;
using Owl.Attributes;
using Owl.Services.VariableResolvers;

namespace Owl.Models.Variables;

[RegisterToVariableResolver(typeof(DynamicVariableResolver))]
public class DynamicVariable : VariableBase
{
	public Guid RequestNodeId { get; set; }
	public string ParsingPath { get; set; } = string.Empty;
}
