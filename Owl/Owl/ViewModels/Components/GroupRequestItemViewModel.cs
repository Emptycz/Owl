using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Interfaces;
using Owl.States;
using Owl.ViewModels.Models;
using Serilog;

namespace Owl.ViewModels.Components;

public partial class GroupRequestItemViewModel : ViewModelBase
{
    [ObservableProperty] private IRequestNodeState _state;
    [ObservableProperty] private GroupRequestVm _groupRequest;
    [ObservableProperty] private IRequestVm? _selectedRequest;

    public GroupRequestItemViewModel(GroupRequestVm groupRequest)
    {
        _state = RequestNodeState.Instance;
        _state.CurrentHasChanged += OnCurrentStateChanged;
        GroupRequest = groupRequest;
    }

    partial void OnSelectedRequestChanged(IRequestVm? value)
    {
        if (value is null) return;
        RequestNodeState.Instance.Current = value;
    }

    // TODO: We should optimize this somehow, right now this is being invoked every time
    //       state changes but that also means, it runs every time state is changed outside of this component
    private void OnCurrentStateChanged(object? e, IRequestVm value)
    {
        if (GroupRequest.Children.Any(c => c.Id == value.Id))
        {
            SelectedRequest = value;
            Log.Debug($"State WAS FOUND and {SelectedRequest} changed to: {value.Name}");
            return;
        }

        Log.Warning("TODO: Optimize nulling the GroupRequestItemViewModel state!");
        SelectedRequest = null;
    }
}
