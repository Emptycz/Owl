using System;

namespace Owl.Models.Variables;

public interface IVariable
{
	public Guid Id { get; init; }
	public string Key { get; set; }
}
