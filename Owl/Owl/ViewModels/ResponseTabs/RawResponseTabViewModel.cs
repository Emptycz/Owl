using CommunityToolkit.Mvvm.ComponentModel;
using Owl.States;

namespace Owl.ViewModels.ResponseTabs;

public partial class RawResponseTabViewModel : ViewModelBase
{
    private readonly IRequestNodeState _requestNodeState;
    [ObservableProperty] private string _response;

    public RawResponseTabViewModel(IRequestNodeState state)
    {
        _requestNodeState = state;
        Response = state.Current?.Response?.Content.ReadAsStringAsync().Result ?? string.Empty;
        _requestNodeState.CurrentHasChanged += (_, node) =>
            Response = node.Response?.Content.ReadAsStringAsync().Result ?? string.Empty;
    }
}
