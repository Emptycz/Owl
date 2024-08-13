using System.Collections.Generic;
using System.Text;
using System.Web;
using Owl.Models;

namespace Owl.Services;

public static class HttpClientParamsBuilder
{
    public static string BuildParams(IEnumerable<RequestParameter> parameters)
    {
        var builder = new StringBuilder();
        foreach (RequestParameter parameter in parameters)
        {
            builder.Append($"{parameter.Key}={parameter.Value}&");
        }

        // string encodedUrl = HttpUtility.UrlEncode(builder.ToString().TrimEnd('&'));
        string encodedUrl = builder.ToString();
        return string.IsNullOrEmpty(encodedUrl) ? string.Empty : "?" + encodedUrl;
    }
}
