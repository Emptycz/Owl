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

    public DynamicVariableResolver(DynamicVariable variable)
    {
        _variable = variable;
        _requestNodeState = RequestNodeState.Instance;
    }

    public string Resolve()
    {
        if (_requestNodeState.Current is null) throw new ArgumentException("No request node is selected in the global RequestNodeState");
        var node = _requestNodeState.Current.ToRequest();
        if (node is not HttpRequest referenceNode) throw new ArgumentException($"Dynamic variable resolve is trying to reference against nonHttpRequest node. The node type is: {_requestNodeState.Current.Type}");

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
