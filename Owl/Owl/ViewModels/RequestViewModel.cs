using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Enums;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.Services;
using Owl.Services.VariableResolvers;
using Owl.States;
using Owl.ViewModels.Models;
using Owl.Views.RequestTabs;
using Owl.Views.ResponseTabs;
using Utils;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Owl.ViewModels;

public partial class RequestViewModel : ViewModelBase
{
    [ObservableProperty] private IRequestNodeState _requestState;
    [ObservableProperty] private string _selectedRequestUrl = string.Empty;
    [ObservableProperty] private string _selectedRequestMethod = "GET";

    [ObservableProperty] private ObservableCollection<RequestNodeVm> _requests = [];
    [ObservableProperty] private string[] _methods = ["GET", "POST", "PUT", "UPDATE", "DELETE"];

    [ObservableProperty] private string _responseSize = string.Empty;
    [ObservableProperty] private HttpStatusCode? _responseStatus;
    [ObservableProperty] private string _responseTime;

    [ObservableProperty] private string _search = string.Empty;

    [ObservableProperty] private int _selectedTabIndex;
    [ObservableProperty] private UserControl _tabContentControl;
    [ObservableProperty] private UserControl _responseContent;

    private CancellationTokenSource _cancellationTokenSource = new();

    private readonly IRequestNodeRepository _nodeRepository;
    private readonly HttpClientService _httpClientService = new();
    private readonly IEnvironmentState _environmentState;
    private readonly IVariableResolverFactory _variableResolverFactory;


    public RequestViewModel(IRequestNodeRepository nodeNodeRepository, IRequestNodeState state,
        IVariableResolverFactory variableResolver, IEnvironmentState envState)
    {
        _nodeRepository = nodeNodeRepository;
        _variableResolverFactory = variableResolver;
        _environmentState = envState;
        ResponseTime = "0 ms";
        RequestState = state;

        Requests = new ObservableCollection<RequestNodeVm>(_nodeRepository.GetAll().Select(r => new RequestNodeVm(r)));
        state.Current = Requests.FirstOrDefault();
        state.CurrentHasChanged += OnRequestHasChanged;

        SelectedTabIndex = state.Current is not null ? 0 : -1;
        TabContentControl = GetTabControl(SelectedTabIndex);
        ResponseContent = GetResponseControl(state.Current?.Response);
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
        Requests = new ObservableCollection<RequestNodeVm>(_nodeRepository
            .Find((x) => x.Name.Contains(value))
            .Select(r => new RequestNodeVm(r)));
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
    private void SwitchTab(string tabIndex)
    {
        SelectedTabIndex = int.Parse(tabIndex);
    }

    [RelayCommand]
    private async Task SendRequest()
    {
        if (RequestState.Current is null) return;

        // TODO: Maybe we want to use Struct instead of Class?
        RequestNode request = RequestState.Current.Clone();

        float responseTime = 0;
        var stopwatch = new Stopwatch();

        var requestVariables = VariableFinder.ExtractVariables(request);
        foreach (FoundVariable foundVariable in requestVariables)
        {
            var variable = _environmentState.GetVariables().SingleOrDefault(x => x.Key == foundVariable.Key);
            if (variable is null)
            {
                Console.WriteLine($"Variable {foundVariable.Key} not found");
                return;
            }

            string resolvedVariableValue = _variableResolverFactory.GetResolver(variable).Resolve();
            // TODO: Move this to a separate method
            switch (foundVariable.Location)
            {
                case FoundVariableLocation.Url:
                    request.Url =
                        request.Url?.Replace($"{{{{ .{variable.Key} }}}}", resolvedVariableValue);
                    break;
                case FoundVariableLocation.Body:
                    request.Body =
                        request.Body?.Replace($"{{{{ .{variable.Key} }}}}", resolvedVariableValue);
                    break;
                case FoundVariableLocation.Header:
                    request.Headers = request.Headers.Select(header =>
                    {
                        header.Value = header.Value.Replace($"{{{{ .{variable.Key} }}}}", resolvedVariableValue);
                        return header;
                    }).ToList();
                    break;
                case FoundVariableLocation.Parameter:
                    request.Parameters = request.Parameters.Select(param =>
                    {
                        param.Value = param.Value.Replace($"{{{{ .{variable.Key} }}}}", resolvedVariableValue);
                        return param;
                    }).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            stopwatch.Start();
            ResponseContent = new ProcessingResponseTab(_cancellationTokenSource);

            HttpResponseMessage? responseMessage;
            switch (RequestState.Current.Method)
            {
                case "GET":
                    responseMessage =
                        await _httpClientService.GetAsync(request, _cancellationTokenSource.Token);
                    break;

                case "POST":
                    responseMessage =
                        await _httpClientService.PostAsync(request, _cancellationTokenSource.Token);
                    break;

                // Add other cases for PUT, DELETE, etc., as needed

                default:
                    responseTime = 0;
                    throw new ArgumentOutOfRangeException(nameof(request.Method),
                        $"Unsupported method type: {request.Method}");
            }

            stopwatch.Stop();
            responseTime = (float)stopwatch.Elapsed.TotalMilliseconds;

            SetResponse(responseMessage);

            ResponseContent = GetResponseControl(request.Response);
            ResponseTime = TimeCalc.CalculateTime(responseTime);
            ResponseStatus = responseMessage.StatusCode;
        }
        catch (OperationCanceledException ex)
        {
            stopwatch.Stop();
            ResponseTime = TimeCalc.CalculateTime(responseTime);
            ResponseStatus = HttpStatusCode.RequestTimeout;
            ResponseContent = new ErrorResponseTab(ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            ResponseTime = string.Empty;

            // Handle any exceptions, such as network errors or JSON parsing errors
            Console.WriteLine($"Error sending request: {ex.Message}");
            ResponseTime = TimeCalc.CalculateTime(responseTime);
            ResponseStatus = HttpStatusCode.InternalServerError;
            ResponseContent = new ErrorResponseTab(ex.Message);
        }
    }

    private void SetResponse(HttpResponseMessage response)
    {
        ResponseSize = SizeCalc.CalculateSize(response.Content.ReadAsStringAsync().Result, Encoding.ASCII).ToString();
        if (RequestState.Current is null) return;

        // TODO: This should be Variable business logic implementation
        string json = response.Content.ReadAsStringAsync().Result;
        var parsedJson = JsonSerializer.Deserialize<JsonElement>(json);

        // Example path provided by the user
        const string userPath = "authentication.strategy"; // Example string path

        // Extract value based on string path
        object? valueFromStringPath = JsonTraverser.TraverseJson(parsedJson, userPath);
        Console.WriteLine(valueFromStringPath); // Output: value at the specified path

        RequestState.Current.Response = response;
    }

    private UserControl GetResponseControl(HttpResponseMessage? response) =>
        response?.Content.Headers.ContentType?.MediaType switch
        {
            "application/xml" => new RawResponseTab(RequestState),
            "application/json" => new JsonResponseTab(RequestState),
            _ => new RawResponseTab(RequestState),
        };

    private UserControl GetTabControl(int tabIndex)
    {
        return tabIndex switch
        {
            0 => new ParamsTab(RequestState, _nodeRepository),
            1 => new BodyTab(RequestState, _nodeRepository),
            2 => new AuthTab(RequestState, _nodeRepository),
            _ => new NoRequestSelectedTab()
        };
    }
}
