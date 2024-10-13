using Owl.Models.Variables;

namespace Owl.Services.VariableResolvers;


public partial class VariableResolverFactory : IVariableResolverFactory
{
	public IVariableResolver GetResolver(IVariable variable)
	{
		// Use generated source code for mapping
		return _getResolver(variable);
	}
}
