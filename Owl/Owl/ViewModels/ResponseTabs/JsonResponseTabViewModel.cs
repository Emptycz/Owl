using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owl.Models;
using Owl.States;

namespace Owl.ViewModels.ResponseTabs;

public partial class JsonResponseTabViewModel : ViewModelBase
{
    [ObservableProperty] private string? _response;

    private readonly ISelectedNodeState _selectedNodeState;

    public JsonResponseTabViewModel(ISelectedNodeState selectedNodeState)
    {
        _selectedNodeState = selectedNodeState;
        _selectedNodeState.CurrentHasChanged += (_, node) => Response = ParseResponse(node);

        Response = ParseResponse(selectedNodeState.Current);
    }

    private static string ParseResponse(RequestNode? node)
    {
        if (node is null || node.Response is null) return string.Empty;
        string content = node.Response.Content.ReadAsStringAsync().Result;

        return JToken.Parse(content).ToString(Formatting.Indented);
    }
}
