using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Owl.Models;

namespace Owl.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient = new();

    public async Task<HttpResponseMessage> GetAsync(RequestNode node, CancellationToken cancellationToken = default)
    {
        if (node.Auth is not null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(node.Auth.Scheme, node.Auth.Token);
        }

        try
        {
            string url = node.Url + HttpClientParamsBuilder.BuildParams(node.Parameters.Where(p => p.IsEnabled));
            var res = await _httpClient.GetAsync(url, cancellationToken);
            return res;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (UriFormatException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<HttpResponseMessage> PostAsync(RequestNode node, CancellationToken cancellationToken = default)
    {
        var content = new StringContent(node.Body, Encoding.UTF8, "application/json");

        if (node.Auth is not null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(node.Auth.Scheme, node.Auth.Token);
        }

        try
        {
            string url = node.Url + HttpClientParamsBuilder.BuildParams(node.Parameters.Where(p => p.IsEnabled));
            return await _httpClient.PostAsync(url, content, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("[HttpRequestException]: {0}", ex.Message);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (UriFormatException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
