using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Owl.Enums;
using Owl.Models;
using Owl.ViewModels.Models;

namespace Owl.Services;

public record FoundVariable(string Key, FoundVariableLocation Location);

public static partial class VariableFinder
{
	private const string VariablePattern = @"\{\{\s*\.(\w+)\s*\}\}";
	private static readonly Regex VariableRegex = CompiledVariableRegex();

	public static bool HasVariable(RequestNode node)
	{
		return HasVariable(node.Body) ||
		       HasVariable(node.Url) ||
		       node.Headers.Any(rh => VariableRegex.IsMatch(rh.Key) || VariableRegex.IsMatch(rh.Value)) ||
		       node.Parameters.Any(p => VariableRegex.IsMatch(p.Key) || VariableRegex.IsMatch(p.Value));
	}

	public static bool HasVariable(string? content) => !string.IsNullOrEmpty(content) && VariableRegex.IsMatch(content);

	public static IEnumerable<FoundVariable> ExtractVariables(RequestNode node)
	{
		var variables = new List<FoundVariable>();

		variables.AddRange(ExtractVariables(node.Body).Select(v => new FoundVariable(v, FoundVariableLocation.Body)));
		variables.AddRange(ExtractVariables(node.Url).Select(v => new FoundVariable(v, FoundVariableLocation.Url)));
		variables.AddRange(node.Headers.SelectMany(rh =>
			ExtractVariables(rh.Key).Select(v => new FoundVariable(v, FoundVariableLocation.Header))
				.Concat(ExtractVariables(rh.Value).Select(v => new FoundVariable(v, FoundVariableLocation.Header)))));
		variables.AddRange(node.Parameters.SelectMany(p =>
			ExtractVariables(p.Key).Select(v => new FoundVariable(v, FoundVariableLocation.Parameter))
				.Concat(ExtractVariables(p.Value).Select(v => new FoundVariable(v, FoundVariableLocation.Parameter)))));

		return variables;
	}

	public static IEnumerable<FoundVariable> ExtractVariables(RequestNodeVm node)
	{
		var variables = new List<FoundVariable>();

		variables.AddRange(ExtractVariables(node.Body).Select(v => new FoundVariable(v, FoundVariableLocation.Body)));
		variables.AddRange(ExtractVariables(node.Url).Select(v => new FoundVariable(v, FoundVariableLocation.Url)));
		variables.AddRange(node.Headers.SelectMany(rh =>
			ExtractVariables(rh.Key).Select(v => new FoundVariable(v, FoundVariableLocation.Header))
				.Concat(ExtractVariables(rh.Value).Select(v => new FoundVariable(v, FoundVariableLocation.Header)))));
		variables.AddRange(node.Parameters.SelectMany(p =>
			ExtractVariables(p.Key).Select(v => new FoundVariable(v, FoundVariableLocation.Parameter))
				.Concat(ExtractVariables(p.Value).Select(v => new FoundVariable(v, FoundVariableLocation.Parameter)))));

		return variables;
	}

	public static IEnumerable<string> ExtractVariables(string[] content)
	{
		return content.SelectMany(ExtractVariables);
	}

	public static IEnumerable<string> ExtractVariables(string? content)
	{
		if (string.IsNullOrEmpty(content)) yield break;

		var matches = VariableRegex.Matches(content);
		foreach (Match match in matches)
		{
			yield return match.Groups[1].Value;
		}
	}

	[GeneratedRegex(VariablePattern, RegexOptions.Compiled)]
	private static partial Regex CompiledVariableRegex();
}
