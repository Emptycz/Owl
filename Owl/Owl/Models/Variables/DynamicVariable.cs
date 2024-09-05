using System;

namespace Owl.Models.Variables;

public class DynamicVariable : VariableBase
{
	public Guid RequestNodeId { get; set; }
	public string ParsingPath { get; set; } = string.Empty;
}
