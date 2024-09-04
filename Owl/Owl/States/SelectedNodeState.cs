using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Models;
using Owl.ViewModels.Models;

namespace Owl.States;

public interface ISelectedNodeState
{
    RequestNodeVm? Current { get; set; }
    event EventHandler<RequestNodeVm>? CurrentHasChanged;
    event EventHandler<bool> RequestedRefresh;

    void OnCurrentHasChanged(RequestNodeVm? node);
}

public partial class SelectedNodeState : ObservableObject, ISelectedNodeState
{
    [ObservableProperty] private RequestNodeVm? _current;
    public event EventHandler<RequestNodeVm>? CurrentHasChanged;
    public event EventHandler<bool>? RequestedRefresh;

    partial void OnCurrentChanged(RequestNodeVm? value)
    {
        OnCurrentHasChanged(value);
    }

    public void OnCurrentHasChanged(RequestNodeVm? node)
    {
        CurrentHasChanged?.Invoke(this, node!);
    }
}
