using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Owl.Models;
using Owl.Repositories.Variable;

namespace Owl.Services;

public interface IVariableResolver
{
    bool HasVariable(RequestNode node);
    bool HasVariable(string? content);
    void ResolveVariables(ref string content);
    RequestNode ResolveVariables(RequestNode node);
    string ResolveVariables(string? content);
    IEnumerable<string> ExtractVariables(string? content);
}

public partial class DbVariableResolver : IVariableResolver
{
    private const string VariablePattern = @"\{\{\s*\.(\w+)\s*\}\}";
    private static readonly Regex VariableRegex = CompiledVariableRegex();
    private readonly IVariableRepository _repository;

    public DbVariableResolver(IVariableRepository repo)
    {
        _repository = repo;
    }

    public bool HasVariable(RequestNode node)
    {
        return HasVariable(node.Body) ||
               HasVariable(node.Url) ||
               node.Headers.Any(rh => VariableRegex.IsMatch(rh.Key) || VariableRegex.IsMatch(rh.Value)) ||
               node.Parameters.Any(p => VariableRegex.IsMatch(p.Key) || VariableRegex.IsMatch(p.Value));
    }

    public bool HasVariable(string? content) => !string.IsNullOrEmpty(content) && VariableRegex.IsMatch(content);

    public IEnumerable<string> ExtractVariables(string? content)
    {
        if (string.IsNullOrEmpty(content)) yield break;

        var matches = VariableRegex.Matches(content);
        foreach (Match match in matches)
        {
            yield return match.Groups[1].Value;
        }
    }

    public string ResolveVariables(string? content)
    {
        if (string.IsNullOrEmpty(content)) return string.Empty;

        var variables = ExtractVariables(content);
        var variableValues = _repository.Find(x => variables.Contains(x.Key));

        return VariableRegex.Replace(content, match =>
        {
            string variableName = match.Groups[1].Value;
            var res = variableValues.FirstOrDefault(v => v.Key == variableName);
            // TODO: This whole thing is just a prototype
            // return res is null ? variableName : res.Value;
            return variableName;
        });
    }

    public void ResolveVariables(ref RequestNode node)
    {
        node.Body = ResolveVariables(node.Body);
        node.Url = ResolveVariables(node.Url);

        foreach (var header in node.Headers)
        {
            header.Key = ResolveVariables(header.Key);
            header.Value = ResolveVariables(header.Value);
        }

        foreach (var param in node.Parameters)
        {
            param.Key = ResolveVariables(param.Key);
            param.Value = ResolveVariables(param.Value);
        }
    }

    public RequestNode ResolveVariables(RequestNode node)
    {
        ResolveVariables(ref node);
        return node;
    }

    public void ResolveVariables(ref string content)
    {
        content = ResolveVariables(content);
    }

    [GeneratedRegex(VariablePattern, RegexOptions.Compiled)]
    private static partial Regex CompiledVariableRegex();
}
