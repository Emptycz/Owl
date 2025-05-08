using System.Collections.Generic;
using Owl.Models.Variables;

namespace Owl.Services;

public static class VariablesManager
{
    /// <summary>
    /// Key is the variable name
    /// Value is a list of resolved variable for multiple environments
    /// </summary>
    private static Dictionary<string, List<ResolvedVariableTuple>> Variables { get; } = new();

    public static string? GetVariableValue(string key, string? environment)
    {
        if (!Variables.TryGetValue(key, out var variable)) return null;

        var resolvedVariable = variable.Find(v => v.EnvironmentName == environment);
        return resolvedVariable.Value;
    }

    public static bool TryGetVariableValue(string key, string? environment, out string? value)
    {
        value = null;
        if (!Variables.TryGetValue(key, out var variable)) return false;

        var resolvedVariable = variable.Find(v => v.EnvironmentName == environment);
        if (resolvedVariable.Value is null) return false;

        value = resolvedVariable.Value;
        return true;
    }

    public static bool AddVariables(IEnumerable<IVariable> variables, string? environment)
    {
        foreach (var variable in variables)
        {
            if (!AddVariable(variable.Key, environment)) return false;
        }

        return true;
    }

    public static bool AddVariable(string key, string? environment)
    {
        if (!Variables.TryGetValue(key, out var variableDocument))
        {
            Variables.Add(key, [new ResolvedVariableTuple { EnvironmentName = environment }]);
            return true;
        }

        bool foundVariable = variableDocument.Exists(v => v.EnvironmentName == environment);
        if (foundVariable) return true;

        variableDocument.Add(new ResolvedVariableTuple { EnvironmentName = environment });
        return true;
    }

    public static bool ResolveVariable(string key, string value, string? environment)
    {
        if (!Variables.TryGetValue(key, out var variableDocument)) return false;

        var resolvedVariable = variableDocument.Find(v => v.EnvironmentName == environment);

        resolvedVariable.Value = value;
        return true;
    }

    public static bool RemoveVariable(string key, string? environment)
    {
        return Variables.TryGetValue(key, out var variableDocument) &&
               variableDocument.Remove(variableDocument.Find(v => v.EnvironmentName == environment));
    }
}


public record struct ResolvedVariableTuple
{
    public string? Value { get; set; }
    public string? EnvironmentName { get; init; }

    public ResolvedVariableTuple(string? value, string? environment)
    {
        Value = value;
        EnvironmentName = environment;
    }
}
