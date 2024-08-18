using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.States;

namespace Owl.ViewModels.RequestTabs;

public partial class BodyTabViewModel : ViewModelBase
{
    [ObservableProperty] private string _body = string.Empty;
    private readonly IRequestNodeRepository _repository;
    private readonly ISelectedNodeState _state;

    public BodyTabViewModel(ISelectedNodeState state, IRequestNodeRepository repo)
    {
        _repository = repo;
        _state = state;
        Body = state.Current?.Body ?? string.Empty;

        _state.CurrentHasChanged += OnSelectedRequestHasChanged;
    }

    private void OnSelectedRequestHasChanged(object? e, RequestNode node)
    {
        Body = node.Body;
    }

    partial void OnBodyChanged(string? value)
    {
        if (_state.Current is null) return;
        _state.Current.Body = value ?? string.Empty;

        _repository.Update(_state.Current);
    }
}
