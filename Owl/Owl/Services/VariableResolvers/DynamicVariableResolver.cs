using System;
using System.Text.Json;
using Owl.Models;
using Owl.Models.Variables;
using Owl.States;

namespace Owl.Services.VariableResolvers;

public class DynamicVariableResolver : IVariableResolver
{
    private readonly DynamicVariable _variable;
    private readonly IRequestNodeState _requestNodeState;

    public DynamicVariableResolver(DynamicVariable variable, IRequestNodeState requestNodeState)
    {
        _variable = variable;
        _requestNodeState = requestNodeState;
    }

    public string Resolve()
    {
        RequestNode? referenceNode = _requestNodeState.Current;
        if (referenceNode is null)
        {
            // TODO: This might be a valid use-case, think of a better way to handle this
            throw new NullReferenceException("Reference node was not found");
        }

        if (referenceNode.Response is null)
        {
            // TODO: This might be a valid use-case, think of a better way to handle this
            throw new NullReferenceException("Reference node response was not found");
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
