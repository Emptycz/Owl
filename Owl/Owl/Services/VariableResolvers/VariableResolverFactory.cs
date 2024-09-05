using System;
using Microsoft.Extensions.DependencyInjection;
using Owl.Models.Variables;
using Owl.Repositories.RequestNode;

namespace Owl.Services.VariableResolvers;

public interface IVariableResolverFactory
{
	public IVariableResolver GetResolver(IVariable variable);
}

public class VariableResolverFactory : IVariableResolverFactory
{
	private readonly IRequestNodeRepository _requestNodeRepository;

	public VariableResolverFactory(IRequestNodeRepository requestNodeRepository)
	{
		_requestNodeRepository = requestNodeRepository;
	}

	public IVariableResolver GetResolver(IVariable variable)
	{
		return variable switch
		{
			StaticVariable staticVariable => new StaticVariableResolver(staticVariable),
			DynamicVariable dynamicVariable => new DynamicVariableResolver(dynamicVariable, _requestNodeRepository),
			_ => throw new ArgumentException("Unknown variable type")
		};
	}
}
