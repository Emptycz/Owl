using CommunityToolkit.Mvvm.ComponentModel;
using Owl.States;

namespace Owl.ViewModels.ResponseTabs;

public partial class RawResponseTabViewModel : ViewModelBase
{
    private readonly ISelectedNodeState _selectedNodeState;
    [ObservableProperty] private string _response;

    public RawResponseTabViewModel(ISelectedNodeState state)
    {
        _selectedNodeState = state;
        Response = state.Current?.Response?.Content.ReadAsStringAsync().Result ?? string.Empty;
        _selectedNodeState.CurrentHasChanged += (_, node) =>
            Response = node.Response?.Content.ReadAsStringAsync().Result ?? string.Empty;
    }
}
