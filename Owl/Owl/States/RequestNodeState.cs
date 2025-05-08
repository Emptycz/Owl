using System;
using System.ComponentModel;
using Owl.Interfaces;

namespace Owl.States;

public interface IRequestNodeState
{
    IRequestVm? Current { get; set; }
    event EventHandler<IRequestVm>? CurrentHasChanged;

    void OnCurrentChanged(IRequestVm? node);
}

public class RequestNodeState : INotifyPropertyChanged, IRequestNodeState {

    private IRequestVm? _current;
    public IRequestVm? Current
    {
        get => _current;
        set
        {
            if (_current == value) return;

            _current = value;
            OnPropertyChanged(nameof(Current));
            OnCurrentChanged(value);
        }
    }

    private static readonly RequestNodeState _instance = new();
    public static RequestNodeState Instance => _instance;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<IRequestVm>? CurrentHasChanged;


    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void OnCurrentChanged(IRequestVm? node)
    {
        CurrentHasChanged?.Invoke(this, node!);
    }
}
