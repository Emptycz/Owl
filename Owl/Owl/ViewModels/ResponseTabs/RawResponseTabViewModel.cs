using CommunityToolkit.Mvvm.ComponentModel;
using Owl.States;
using Owl.ViewModels.Models;

namespace Owl.ViewModels.ResponseTabs;

public partial class RawResponseTabViewModel : ViewModelBase
{
    private readonly IRequestNodeState _requestNodeState;
    [ObservableProperty] private string _response;

    public RawResponseTabViewModel(IRequestNodeState state)
    {
        _requestNodeState = state;
        if (_requestNodeState.Current is not HttpRequestVm vm) return;

        Response = vm.Response?.Content.ReadAsStringAsync().Result ?? string.Empty;
        _requestNodeState.CurrentHasChanged += (_, node) =>
            Response = vm.Response?.Content.ReadAsStringAsync().Result ?? string.Empty;
    }
}
