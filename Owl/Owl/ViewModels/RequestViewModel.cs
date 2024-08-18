using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owl.Contexts;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Variable;
using Owl.Services;
using Owl.States;
using Owl.Views.RequestTabs;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Owl.ViewModels;

public partial class RequestViewModel : ViewModelBase
{
    [ObservableProperty] private ISelectedNodeState _requestState;
    [ObservableProperty] private string _selectedRequestUrl = string.Empty;
    [ObservableProperty] private string _selectedRequestMethod = "GET";

    [ObservableProperty] private ObservableCollection<RequestNode> _requests = [];
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

        state.Current = _requests.FirstOrDefault();
        RequestState = state;
        state.CurrentHasChanged += OnRequestHasChanged;

        SelectedTabIndex = state.Current is not null ? 0 : -1;
        TabContentControl = GetTabControl(SelectedTabIndex);
    }

    private void OnRequestHasChanged(object? e, RequestNode? node)
    {
        if (node is null)
        {
            SelectedTabIndex = -1;
            TabContentControl = GetTabControl(SelectedTabIndex);
            return;
        }

        if (SelectedTabIndex != -1) return;

        SelectedTabIndex = 0;
        TabContentControl = GetTabControl(SelectedTabIndex);
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        TabContentControl = GetTabControl(value);
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

        // TOOD: This is only for testing purposes, pass this via DI container
        IVariableResolver resolver = new DbVariableResolver(new LiteDbVariableRepository(new LiteDbContext()));

        Console.WriteLine("Body {0} has var: {1}", RequestState.Current.Body,
            resolver.HasVariable(RequestState.Current.Body));
        Console.WriteLine("Body has variables: {0}",
            JsonSerializer.Serialize(resolver.ExtractVariables(RequestState.Current.Body)));
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
                    responseMessage = await _httpClientService.PostAsync(RequestState.Current);
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

    private UserControl GetTabControl(int tabIndex)
    {
        switch (tabIndex)
        {
            default:
            case -1:
                return new NoRequestSelectedTab();

            case 0:
                return new ParamsTab(RequestState, _nodeRepository);

            case 1:
                return new BodyTab(RequestState, _nodeRepository);

            case 2:
                return new AuthTab(RequestState, _nodeRepository);
        }
    }
}
