using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Enums;
using Owl.Models;
using Owl.Models.Variables;
using Owl.Repositories.RequestNode;
using Owl.Services;
using Owl.Services.VariableResolvers;
using Owl.States;
using Owl.ViewModels.Models;
using Owl.Views.RequestTabs;
using Owl.Views.ResponseTabs;
using Utils;

namespace Owl.ViewModels;

public partial class RequestViewModel : ViewModelBase
{
    [ObservableProperty] private IRequestNodeState _requestState;
    [ObservableProperty] private ObservableCollection<RequestNodeVm> _requests = [];

    [ObservableProperty] private HttpRequestType[] _methods =
    [
        HttpRequestType.Get,
        HttpRequestType.Post,
        HttpRequestType.Put,
        HttpRequestType.Update,
        HttpRequestType.Delete,
    ];

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
            .Find(x => x.Name.Contains(value))
            .Select(r => new RequestNodeVm(r)));
    }

    [RelayCommand]
    private void AddHeader()
    {
        RequestState.Current?.Headers.Add(new RequestHeader());
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

        var requestVariables = VariableFinder.ExtractVariables(request);
        foreach (FoundVariable foundVariable in requestVariables)
        {
            string? resolvedVariableValue =
                VariablesManager.GetVariableValue(foundVariable.Key, _environmentState.Current?.Name);

            // TODO: Do something when variable was not found (maybe throw or return and render error)
            if (resolvedVariableValue is null) continue;

            request.ResolveVariable(foundVariable, resolvedVariableValue);
        }

        var stopwatch = new Stopwatch();
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            ResponseContent = new ProcessingResponseTab(_cancellationTokenSource);

            stopwatch.Start();
            HttpResponseMessage responseMessage = RequestState.Current.Method switch
            {
                HttpRequestType.Get => await _httpClientService.GetAsync(request, _cancellationTokenSource.Token),
                HttpRequestType.Post => await _httpClientService.PostAsync(request, _cancellationTokenSource.Token),
                _ => throw new ArgumentOutOfRangeException(nameof(request.Method),
                    $"Unsupported method type: {request.Method}")
            };
            stopwatch.Stop();

            SetResponse(responseMessage);
            ResolveReferencingVariables();

            ResponseContent = GetResponseControl(responseMessage);
            ResponseTime = TimeFormatter.ToTimeElapsed(stopwatch.Elapsed);
            ResponseStatus = responseMessage.StatusCode;
        }
        catch (OperationCanceledException ex)
        {
            stopwatch.Stop();
            ResponseTime = TimeFormatter.ToTimeElapsed(stopwatch.Elapsed);
            ResponseStatus = HttpStatusCode.RequestTimeout;
            ResponseContent = new ErrorResponseTab(ex.Message);
        }
        catch (Exception ex)
        {
            ResponseTime = string.Empty;

            Console.WriteLine(ex);
            // Handle any exceptions, such as network errors or JSON parsing errors
            Console.WriteLine($"Error sending request: {ex.Message}");
            ResponseStatus = HttpStatusCode.InternalServerError;
            ResponseContent = new ErrorResponseTab(ex.Message);
        }
    }

    private void SetResponse(HttpResponseMessage response)
    {
        ResponseSize = SizeCalc.CalculateSize(response.Content.ReadAsStringAsync().Result, Encoding.ASCII).ToString();
        if (RequestState.Current is null) return;
        RequestState.Current.Response = response;
    }

    private void ResolveReferencingVariables()
    {
        if (RequestState.Current is null) return;

        // Resolve dynamic variables
        var variables = _environmentState.Current?.Variables.ToList();
        if (variables is null || !variables.Any()) return;

        var dynamicVariables = variables.OfType<DynamicVariable>().ToList();
        if (!dynamicVariables.Any()) return;

        var referencedVariables = dynamicVariables.FindAll(x => x.RequestNodeId == RequestState.Current.Id);
        foreach (var variable in referencedVariables)
        {
            string value = _variableResolverFactory.GetResolver(variable).Resolve();
            VariablesManager.ResolveVariable(variable.Key, value, _environmentState.Current?.Name);
        }
    }

    private UserControl GetResponseControl(HttpResponseMessage? response)
    {
        return response?.Content.Headers.ContentType?.MediaType switch
        {
            "application/xml" => new RawResponseTab(RequestState),
            "application/json" => new JsonResponseTab(RequestState),
            // TODO: We can use system default webView to try to render the HTML page
            // "text/html" => new HtmlResponseTab(),
            _ => new RawResponseTab(RequestState),
        };
    }

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
