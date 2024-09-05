using System;

namespace Owl.Models.Variables;

public class VariableBase : IVariable
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public string Key { get; set; } = string.Empty;
}
