using System.Collections.Generic;

namespace Owl.Services;

public static class VariableManager
{
    private static Dictionary<string, List<ResolvedVariableTuple>> Variables { get; set; } = new();

    public static string GetVariableValue(string key, string? environment = "__GLOBAL__")
    {
        if (!Variables.TryGetValue(key, out var variable)) return string.Empty;

        var resolvedVariable = variable.Find(v => v.EnvironmentName == environment);
        return resolvedVariable.Value;
    }

    public static bool AddVariable(string key, string value, string environment)
    {
        if (Variables.TryGetValue(key, out var variableDocument))
        {
            bool foundVariable = variableDocument.Exists(v => v.EnvironmentName == environment);
            if (foundVariable)
            {
                if (!RemoveVariable(key, environment)) return false;
            }

            variableDocument.Add(new ResolvedVariableTuple(value, environment));
            return true;
        }

        Variables.Add(key, [new ResolvedVariableTuple(value, environment)]);
        return true;
    }

    public static bool RemoveVariable(string key, string environment)
    {
        return Variables.TryGetValue(key, out var variableDocument) &&
               variableDocument.Remove(variableDocument.Find(v => v.EnvironmentName == environment));
    }
}


public record struct ResolvedVariableTuple
{
    public string Value { get; set; }
    public string EnvironmentName { get; set; }

    public ResolvedVariableTuple(string value, string environment)
    {
        Value = value;
        EnvironmentName = environment;
    }
}
