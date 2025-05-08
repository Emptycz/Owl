using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Owl.Services;

public static class JsonTraverser
{
    public static string? TraverseJson(JsonElement jsonElement, string path)
    {
        string[] parts = path.Split('.');
        JsonElement currentElement = jsonElement;

        foreach (string part in parts)
        {
            if (currentElement.TryGetProperty(part, out JsonElement nextElement))
            {
                currentElement = nextElement;
            }
            else
            {
                return null;
            }
        }

        return currentElement.ToString();
    }

    public static List<object?> TraverseJsonWithRegex(JsonElement jsonElement, string regexPath)
    {
        var matches = new List<object?>();
        TraverseJsonWithRegexRecursive(jsonElement, regexPath, "", matches);
        return matches;
    }

    private static void TraverseJsonWithRegexRecursive(JsonElement jsonElement, string regexPath, string currentPath, List<object?> matches)
    {
        if (Regex.IsMatch(currentPath, regexPath))
        {
            matches.Add(jsonElement.ValueKind switch
            {
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => jsonElement.ToString()
            });
        }

        if (jsonElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in jsonElement.EnumerateObject())
            {
                TraverseJsonWithRegexRecursive(property.Value, regexPath, $"{currentPath}.{property.Name}".Trim('.'), matches);
            }
        }
        else if (jsonElement.ValueKind == JsonValueKind.Array)
        {
            int index = 0;
            foreach (var item in jsonElement.EnumerateArray())
            {
                TraverseJsonWithRegexRecursive(item, regexPath, $"{currentPath}[{index}]", matches);
                index++;
            }
        }
    }
}
