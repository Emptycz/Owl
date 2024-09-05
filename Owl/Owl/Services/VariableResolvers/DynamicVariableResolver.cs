using Owl.Models.Variables;
using Owl.Repositories.RequestNode;

namespace Owl.Services.VariableResolvers;

public class DynamicVariableResolver : IVariableResolver
{
	private DynamicVariable Variable { get; init; }
	private readonly IRequestNodeRepository _requestNodeRepository;

	public DynamicVariableResolver(DynamicVariable variable, IRequestNodeRepository requestNodeRepository)
	{
		Variable = variable;
		_requestNodeRepository = requestNodeRepository;
	}

	public string Resolve()
	{
		throw new System.NotImplementedException();
	}
}
