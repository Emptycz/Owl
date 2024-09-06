using System;
using System.Text.Json;
using Owl.Models.Variables;
using Owl.Repositories.RequestNode;

namespace Owl.Services.VariableResolvers;

public class DynamicVariableResolver : IVariableResolver
{
    private readonly DynamicVariable _variable;
    private readonly IRequestNodeRepository _requestNodeRepository;

    public DynamicVariableResolver(DynamicVariable variable, IRequestNodeRepository requestNodeRepository)
    {
        _variable = variable;
        _requestNodeRepository = requestNodeRepository;
    }

    public string Resolve()
    {
        var referenceNode = _requestNodeRepository.Get(_variable.RequestNodeId);
        if (referenceNode is null)
        {
            // TODO: This might be a valid use-case, think of a better way to handle this
            throw new NullReferenceException("Reference node was not found");
        }

        string? jsonResponse = referenceNode.Response?.Content.ReadAsStringAsync().Result;
        if (jsonResponse is null)
        {
            throw new Exception("Could not read the NodeRequest response as string");
        }

        string? res = JsonTraverser.TraverseJson(
            JsonSerializer.Deserialize<JsonElement>(jsonResponse),
            _variable.ParsingPath
        );

        if (res is null)
        {
            throw new Exception("Could not resolve the variable");
        }

        return res;
    }
}
