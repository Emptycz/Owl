using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owl.Models;
using Owl.States;
using Owl.ViewModels.Models;

namespace Owl.ViewModels.ResponseTabs;

public partial class JsonResponseTabViewModel : ViewModelBase
{
    [ObservableProperty] private string? _response;

    private readonly IRequestNodeState _requestNodeState;

    public JsonResponseTabViewModel(IRequestNodeState requestNodeState)
    {
        _requestNodeState = requestNodeState;
        _requestNodeState.CurrentHasChanged += (_, node) =>
        {
            if (node is not HttpRequestVm vm) return;
            Response = ParseResponse(vm);
        };

        if (_requestNodeState.Current is not HttpRequestVm vm) return;
        Response = ParseResponse(vm);
    }

    private static string ParseResponse(HttpRequest? node)
    {
        if (node is null || node.Response is null) return string.Empty;
        string content = node.Response.Content.ReadAsStringAsync().Result;

        return JToken.Parse(content).ToString(Formatting.Indented);
    }
}
