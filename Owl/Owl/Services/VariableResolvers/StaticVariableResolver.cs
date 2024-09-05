using Owl.Models.Variables;

namespace Owl.Services.VariableResolvers;

public class StaticVariableResolver : IVariableResolver
{
	private StaticVariable Variable { get; init; }

	public StaticVariableResolver(StaticVariable variable)
	{
		Variable = variable;
	}

	public string Resolve() => Variable.Value;
}

