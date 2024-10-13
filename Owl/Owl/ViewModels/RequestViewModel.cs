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
using Owl.Factories;
using Owl.Interfaces;
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
    [ObservableProperty] private ObservableCollection<IRequestVm> _requests = [];

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
    [ObservableProperty] private HttpRequestVm? _request;

    private CancellationTokenSource _cancellationTokenSource = new();

    private readonly IRequestNodeState _requestState;
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
        _requestState = state;

        if (_requestState.Current is HttpRequestVm vm)
        {
            _request = vm;
        }

        Requests = new ObservableCollection<IRequestVm>(_nodeRepository.GetAll().Select(RequestNodeVmFactory.GetRequestNodeVm));
        state.Current = Requests.FirstOrDefault();
        state.CurrentHasChanged += OnRequestHasChanged;

        SelectedTabIndex = state.Current is not null ? 0 : -1;
        TabContentControl = GetTabControl(SelectedTabIndex);
        ResponseContent = GetResponseControl(Request?.Response);
    }

    private void OnRequestHasChanged(object? e, IRequestVm? node)
    {
        if (node is not HttpRequestVm vm) return;
        Request = vm;
        if (Request is null)
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
        Requests = new ObservableCollection<IRequestVm>(_nodeRepository
            .Find(x => x.Name.Contains(value))
            .Select(RequestNodeVmFactory.GetRequestNodeVm));
    }

    [RelayCommand]
    private void AddHeader()
    {
        Request?.Headers.Add(new RequestHeader());
        // SelectedRequest.Headers.Add(newHeader.Key, newHeader.Value);
        // HeadersCollection.Add(newHeader);
    }

    [RelayCommand]
    private void SelectedRequestPropertyHasChanged()
    {
        if (Request is null) return;

        _nodeRepository.Update(Request);

        var request = Requests.FirstOrDefault(x => x.Id == Request?.Id);
        if (request is null || request is not HttpRequest httpRequest) return;

        // request.Method = RequestState.Current.Method;
    }

    [RelayCommand]
    private void SetBody(string value)
    {
        // FIXME: Custom behavior that added Text Binding support re-triggers the setter and it causes two triggers instead of one
        if (Request is null ||
            (Request.Body == value && !string.IsNullOrEmpty(value))) return;

        Request.Body = value;
        // TODO: Implement debounce to avoid sending request on every key press
        _nodeRepository.Update(Request);
    }

    [RelayCommand]
    private void SwitchTab(string tabIndex)
    {
        SelectedTabIndex = int.Parse(tabIndex);
    }

    [RelayCommand]
    private async Task SendRequest()
    {
        if (Request is null) return;

        // TODO: Maybe we want to use Struct instead of Class?
        HttpRequest httpRequest = Request.Clone();

        var requestVariables = VariableFinder.ExtractVariables(httpRequest);
        foreach (FoundVariable foundVariable in requestVariables)
        {
            string? resolvedVariableValue =
                VariablesManager.GetVariableValue(foundVariable.Key, _environmentState.Current?.Name);

            // TODO: Do something when variable was not found (maybe throw or return and render error)
            if (resolvedVariableValue is null) continue;

            httpRequest.ResolveVariable(foundVariable, resolvedVariableValue);
        }

        var stopwatch = new Stopwatch();
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            ResponseContent = new ProcessingResponseTab(_cancellationTokenSource);

            stopwatch.Start();
            HttpResponseMessage responseMessage = Request.Method switch
            {
                HttpRequestType.Get => await _httpClientService.GetAsync(httpRequest, _cancellationTokenSource.Token),
                HttpRequestType.Post => await _httpClientService.PostAsync(httpRequest, _cancellationTokenSource.Token),
                _ => throw new ArgumentOutOfRangeException(nameof(httpRequest.Method),
                    $"Unsupported method type: {httpRequest.Method}")
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
        if (Request is null) return;
        Request.Response = response;
    }

    private void ResolveReferencingVariables()
    {
        if (Request is null) return;

        // Resolve dynamic variables
        var variables = _environmentState.Current?.Variables.ToList();
        if (variables is null || variables.Count == 0) return;

        var dynamicVariables = variables.OfType<DynamicVariable>().ToList();
        if (dynamicVariables.Count == 0) return;

        var referencedVariables = dynamicVariables.FindAll(x => x.RequestNodeId == Request.Id);
        foreach (var variable in referencedVariables)
        {
            string value = _variableResolverFactory.GetResolver(variable).Resolve();
            VariablesManager.ResolveVariable(variable.Key, value, _environmentState.Current?.Name);
        }
    }

    private UserControl GetResponseControl(HttpResponseMessage? response)
    {
        // TODO: Create source gen mapping for this
        return response?.Content.Headers.ContentType?.MediaType switch
        {
            "application/xml" => new RawResponseTab(_requestState),
            "application/json" => new JsonResponseTab(_requestState),
            // TODO: We can use system default webView to try to render the HTML page
            // "text/html" => new HtmlResponseTab(),
            _ => new RawResponseTab(_requestState),
        };
    }

    private UserControl GetTabControl(int tabIndex)
    {
        return tabIndex switch
        {
            0 => new ParamsTab(_requestState, _nodeRepository),
            1 => new BodyTab(_requestState, _nodeRepository),
            2 => new AuthTab(_requestState, _nodeRepository),
            _ => new NoRequestSelectedTab()
        };
    }
}
