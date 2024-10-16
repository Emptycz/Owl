using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Owl.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient = new();

    public async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");
            
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

    public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, CancellationToken cancellationToken = default)
    {
        try
        {
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