using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Owl.Enums;
using Owl.Models;

namespace Owl.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient = new();

    public async Task<HttpResponseMessage> ProcessRequestAsync(HttpRequest node, CancellationToken cancellationToken)
    {
        if (node.Auth is not null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(node.Auth.Scheme, node.Auth.Token);
        }

        string url = node.Url + HttpClientParamsBuilder.BuildParams(node.Parameters.Where(p => p.IsEnabled));
        var content = node.Body is not null ? new StringContent(node.Body, Encoding.UTF8, "application/json") : null;

        return node.Method switch
        {
            HttpRequestMethod.Get => await _httpClient.GetAsync(url, cancellationToken),
            HttpRequestMethod.Post => await _httpClient.PostAsync(url, content, cancellationToken),
            HttpRequestMethod.Put => await _httpClient.PutAsync(url, content, cancellationToken),
            HttpRequestMethod.Patch => await _httpClient.PatchAsync(url, content, cancellationToken),
            HttpRequestMethod.Delete => await _httpClient.DeleteAsync(url, cancellationToken),
            _ => throw new NotImplementedException($"HTTP method {node.Method} is not implemented.")
        };
    }
}
