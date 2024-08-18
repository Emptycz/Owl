using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Models;

namespace Owl.States;

public interface ISelectedNodeState
{
    RequestNode? Current { get; set; }
    event EventHandler<RequestNode>? CurrentHasChanged;
    event EventHandler<bool> RequestedRefresh;

    void OnCurrentHasChanged(RequestNode? node);
}

public partial class SelectedNodeState : ObservableObject, ISelectedNodeState
{
    [ObservableProperty] private RequestNode? _current;
    public event EventHandler<RequestNode>? CurrentHasChanged;
    public event EventHandler<bool>? RequestedRefresh;

    partial void OnCurrentChanged(RequestNode? value)
    {
        OnCurrentHasChanged(value);
    }

    public void OnCurrentHasChanged(RequestNode? node)
    {
        CurrentHasChanged?.Invoke(this, node!);
    }
}
