using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.ViewModels.Models;

namespace Owl.States;

public interface IRequestNodeState
{
    RequestNodeVm? Current { get; set; }
    event EventHandler<RequestNodeVm>? CurrentHasChanged;

    void OnCurrentHasChanged(RequestNodeVm? node);
}

public partial class RequestNodeState : ObservableObject, IRequestNodeState
{
    [ObservableProperty] private RequestNodeVm? _current;
    public event EventHandler<RequestNodeVm>? CurrentHasChanged;

    partial void OnCurrentChanged(RequestNodeVm? value)
    {
        OnCurrentHasChanged(value);
    }

    public void OnCurrentHasChanged(RequestNodeVm? node)
    {
        CurrentHasChanged?.Invoke(this, node!);
    }
}
