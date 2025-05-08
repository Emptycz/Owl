using Owl.Models.Variables;

namespace Owl.Services.VariableResolvers;

public interface IVariableResolverFactory
{
    public IVariableResolver GetResolver(IVariable variable);
}
