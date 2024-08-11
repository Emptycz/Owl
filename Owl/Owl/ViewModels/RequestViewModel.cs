using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owl.Models;
using Owl.Repositories.RequestNodeRepository;
using Owl.Services;
using Owl.States;
using Owl.Views.RequestTabs;

namespace Owl.ViewModels;

public partial class RequestViewModel : ViewModelBase
{
    [ObservableProperty] private ISelectedNodeState _requestState;
    [ObservableProperty] private string _selectedRequestUrl = string.Empty;
    [ObservableProperty] private string _selectedRequestMethod = "GET";

    [ObservableProperty] private ObservableCollection<RequestNode> _requests;
    [ObservableProperty] private string[] _methods = ["GET", "POST", "PUT", "UPDATE", "DELETE"];

    [ObservableProperty] private string _response = string.Empty;
    [ObservableProperty] private HttpStatusCode? _responseStatus;
    [ObservableProperty] private float _responseTime;

    [ObservableProperty] private string _search = string.Empty;

    [ObservableProperty] private int _selectedTabIndex;
    [ObservableProperty] private UserControl _tabContentControl;

    private readonly IRequestNodeRepository _nodeRepository;
    private readonly HttpClientService _httpClientService = new();


    public RequestViewModel(IRequestNodeRepository nodeNodeRepository, ISelectedNodeState state)
    {
        _nodeRepository = nodeNodeRepository;
        _requests = new ObservableCollection<RequestNode>(_nodeRepository.GetAll());
        RequestState = state;
        RequestState.Current = _requests.FirstOrDefault() ?? new RequestNode { Name = "New Request" };

        TabContentControl = new ParamsTab(RequestState, _nodeRepository);
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        Console.WriteLine("Tab selection just changed {0}!", value);
        switch (value)
        {
            default:
            case 0:
                TabContentControl = new ParamsTab(RequestState, _nodeRepository);
                break;

            case 1:
                TabContentControl = new BodyTab(RequestState);
                break;
        }
    }

    partial void OnSearchChanging(string value)
    {
        Requests = new ObservableCollection<RequestNode>(_nodeRepository.Find((x) => x.Name.Contains(value)));
    }   

    [RelayCommand]
    private void AddHeader()
    {
        if (RequestState.Current is null) return;
        RequestState.Current.Headers.Add(new RequestHeader());
        // SelectedRequest.Headers.Add(newHeader.Key, newHeader.Value);
        // HeadersCollection.Add(newHeader);
    }
    
    [RelayCommand]
    private void SelectedRequestPropertyHasChanged()
    {
        if (RequestState.Current == null) return;

        _nodeRepository.Update(RequestState.Current);

        var request = Requests.FirstOrDefault(x => x.Id == RequestState.Current.Id);
        if (request is null) return;

        request.Method = RequestState.Current.Method;
    }

    [RelayCommand]
    private void SetBody(string value)
    {
        // FIXME: Custom behavior that added Text Binding support re-triggers the setter and it causes two triggers instead of one  
        if (RequestState.Current is null ||
            (RequestState.Current.Body == value && !string.IsNullOrEmpty(value))) return;

        RequestState.Current.Body = value;
        // TODO: Implement debounce to avoid sending request on every key press
        _nodeRepository.Update(RequestState.Current);
    }

    [RelayCommand]
    private async Task SendRequest()
    {
        if (RequestState.Current is null) return;

        ResponseTime = 0;

        var stopwatch = new Stopwatch();

        Console.WriteLine(
            $"Sending request to {RequestState.Current.Url} with method: {RequestState.Current.Method} with body: {RequestState.Current.Body}");

        try
        {
            stopwatch.Start();

            HttpResponseMessage? responseMessage;
            switch (RequestState.Current.Method)
            {
                case "GET":
                    responseMessage = await _httpClientService.GetAsync(RequestState.Current);
                    break;

                case "POST":
                    var content = new StringContent(RequestState.Current.Body, Encoding.UTF8, "application/json");
                    responseMessage = await _httpClientService.PostAsync(RequestState.Current.Url, content);
                    break;

                // Add other cases for PUT, DELETE, etc., as needed

                default:
                    ResponseTime = 0;
                    throw new ArgumentOutOfRangeException(nameof(RequestState.Current.Method),
                        $"Unsupported method type: {RequestState.Current.Method}");
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