using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Interfaces;

namespace Owl.States;

public interface IRequestNodeState
{
    IRequestVm? Current { get; set; }
    event EventHandler<IRequestVm>? CurrentHasChanged;

    void OnCurrentHasChanged(IRequestVm? node);
}

public partial class RequestNodeState : ObservableObject, IRequestNodeState
{
    [ObservableProperty] private IRequestVm? _current;
    public event EventHandler<IRequestVm>? CurrentHasChanged;

    partial void OnCurrentChanged(IRequestVm? value)
    {
        OnCurrentHasChanged(value);
    }

    public void OnCurrentHasChanged(IRequestVm? node)
    {
        CurrentHasChanged?.Invoke(this, node!);
    }
}
