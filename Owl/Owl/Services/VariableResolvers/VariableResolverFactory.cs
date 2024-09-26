using System;
using Owl.Models.Variables;
using Owl.Repositories.RequestNode;
using Owl.States;

namespace Owl.Services.VariableResolvers;

public interface IVariableResolverFactory
{
	public IVariableResolver GetResolver(IVariable variable);
}

public class VariableResolverFactory : IVariableResolverFactory
{
	private readonly IRequestNodeRepository _requestNodeRepository;
	private readonly IRequestNodeState _requestNodeState;

	public VariableResolverFactory(IRequestNodeRepository requestNodeRepository, IRequestNodeState requestNodeState)
	{
		_requestNodeRepository = requestNodeRepository;
		_requestNodeState = requestNodeState;
	}

	public IVariableResolver GetResolver(IVariable variable)
	{
		return variable switch
		{
			StaticVariable staticVariable => new StaticVariableResolver(staticVariable),
			DynamicVariable dynamicVariable => new DynamicVariableResolver(dynamicVariable, _requestNodeState),
			_ => throw new ArgumentException("Unknown variable type: " + variable.GetType().Name)
		};
	}
}
