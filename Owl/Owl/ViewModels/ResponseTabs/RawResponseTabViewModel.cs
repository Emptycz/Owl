using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.States;

namespace Owl.ViewModels.ResponseTabs;

public partial class RawResponseTabViewModel : ViewModelBase
{
    private readonly ISelectedNodeState _selectedNodeState;
    [ObservableProperty] private HttpResponseMessage? _response;

    public RawResponseTabViewModel(ISelectedNodeState state)
    {
        _selectedNodeState = state;
        Response = state.Current?.Response;
        _selectedNodeState.CurrentHasChanged += (_, node) => Response = node.Response;
    }
}
