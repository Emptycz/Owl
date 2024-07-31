using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owl.Contexts;
using Owl.Models;
using Owl.Repositories.RequestNodeRepository;
using Owl.Services;

namespace Owl.ViewModels;

public partial class RequestViewModel : ViewModelBase
{
    [ObservableProperty] private RequestNode? _selectedRequest;
    [ObservableProperty] private string _selectedRequestUrl = string.Empty;
    [ObservableProperty] private string _selectedRequestMethod = "GET";

    [ObservableProperty] private ObservableCollection<RequestNode> _requests;
    [ObservableProperty] private string[] _methods = ["GET", "POST", "PUT", "UPDATE", "DELETE"];

    [ObservableProperty] private string _response = string.Empty;
    [ObservableProperty] private HttpStatusCode? _responseStatus;
    [ObservableProperty] private float _responseTime;

    [ObservableProperty] private string _search = string.Empty;
    
    [ObservableProperty] private int _selectedTabIndex;

    private readonly HttpClientService _httpClientService = new();
    private readonly IRequestNodeRepository _repository;
    

    public RequestViewModel(IRequestNodeRepository nodeRepository)
    {
        _repository = nodeRepository;
        _requests = new ObservableCollection<RequestNode>(_repository.GetAll());
        _selectedRequest = _requests.FirstOrDefault() ?? new RequestNode { Name = "New Request" };
    }

    // public void LoadHeadersIntoCollection()
    // {
    //     HeadersCollection.Clear();
    //     if (SelectedRequest is null) return;
    //     foreach (var header in SelectedRequest.Headers ?? [])
    //     {
    //         HeadersCollection.Add(header);
    //     }
    // }

    partial void OnSelectedTabIndexChanged(int value)
    {
        Console.WriteLine("Tab selection just changed {0}!", value);
    }
    
    partial void OnSearchChanging(string value)
    {
        Requests = new ObservableCollection<RequestNode>(_repository.Find((x) => x.Name.Contains(value)));
    }

    [RelayCommand]
    private void RefreshData(Guid? keepSelected)
    {
        Requests = new ObservableCollection<RequestNode>(_repository.GetAll());
        SelectedRequest = Requests.FirstOrDefault((x) => x.Id == keepSelected);
        OnPropertyChanged(nameof(Requests));
    }   

    [RelayCommand]
    private void AddHeader()
    {
        if (SelectedRequest is null) return;
        SelectedRequest.Headers.Add(new RequestHeader());
        // SelectedRequest.Headers.Add(newHeader.Key, newHeader.Value);
        // HeadersCollection.Add(newHeader);
    }

    partial void OnSelectedRequestChanging(RequestNode? newValue)
    {
        Console.WriteLine("SelectedRequestChanging: {0}", newValue?.Id);
    }

    // [RelayCommand]
    // private void RemoveHeader(KeyValuePair<string, string> header)
    // {
    //     if (SelectedRequest is null) return;
    //     (string? key, _) = header;
    //     SelectedRequest.Headers?.Remove(key);
    //     HeadersCollection.Remove(header);
    // }

    [RelayCommand]
    private void SelectedRequestPropertyHasChanged()
    {
        if (SelectedRequest == null) return;

        _repository.Update(SelectedRequest);
        
        var request = Requests.FirstOrDefault(x => x.Id == SelectedRequest.Id);
        if (request is null) return;
        
        request.Method = SelectedRequest.Method;
        }

    [RelayCommand]
    private void SetBody(string value)
    {
        // FIXME: Custom behavior that added Text Binding support re-triggers the setter and it causes two triggers instead of one  
        if (SelectedRequest is null || (SelectedRequest.Body == value && !string.IsNullOrEmpty(value))) return;

        SelectedRequest.Body = value;
        // TODO: Implement debounce to avoid sending request on every key press
        _repository.Update(SelectedRequest);
    }
    
    [RelayCommand]
    private void AddNode()
    {
        var newNode = new RequestNode
        {
            Name = "New Request",
            Method = "GET",
            Body = string.Empty,
        };

        Requests.Add(_repository.Add(newNode));
    }

    [RelayCommand]
    private void RemoveRequest(RequestNode node)
    {
        _repository.Delete(node.Id);
        Requests.Remove(node);
    }

    [RelayCommand]
    private async Task SendRequest()
    {
        if (SelectedRequest is null) return;

        ResponseTime = 0;

        var stopwatch = new Stopwatch();

        Console.WriteLine(
            $"Sending request to {SelectedRequest.Url} with method: {SelectedRequest.Method} with body: {SelectedRequest.Body}");

        try
        {
            stopwatch.Start();

            HttpResponseMessage? responseMessage;
            switch (SelectedRequest.Method)
            {
                case "GET":
                    responseMessage = await _httpClientService.GetAsync(SelectedRequest.Url);
                    break;

                case "POST":
                    var content = new StringContent(SelectedRequest.Body, Encoding.UTF8, "application/json");
                    responseMessage = await _httpClientService.PostAsync(SelectedRequest.Url, content);
                    break;

                // Add other cases for PUT, DELETE, etc., as needed

                default:
                    ResponseTime = 0;
                    throw new ArgumentOutOfRangeException(nameof(SelectedRequest.Method),
                        $"Unsupported method type: {SelectedRequest.Method}");
            }

            stopwatch.Stop();
            ResponseTime = (float)stopwatch.Elapsed.TotalMilliseconds;

            Response = JToken.Parse(await responseMessage.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
            ResponseStatus = responseMessage.StatusCode;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            ResponseTime = 0;

            // Handle any exceptions, such as network errors or JSON parsing errors
            Console.WriteLine($"Error sending request: {ex.Message}");
            ResponseStatus = HttpStatusCode.InternalServerError;
            Response = ex.Message;
        }
    }
}
