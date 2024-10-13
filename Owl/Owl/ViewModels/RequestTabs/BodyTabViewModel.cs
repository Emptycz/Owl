using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Exceptions;
using Owl.Interfaces;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.States;
using Owl.ViewModels.Models;

namespace Owl.ViewModels.RequestTabs;

public partial class BodyTabViewModel : ViewModelBase
{
    [ObservableProperty] private string _body = string.Empty;
    private readonly IRequestNodeRepository _repository;
    private readonly IRequestNodeState _state;

    public BodyTabViewModel(IRequestNodeState state, IRequestNodeRepository repo)
    {
        _repository = repo;
        _state = state;

        if (state.Current is null) return;

        if (state.Current is not HttpRequestVm httpRequest) throw new InvalidRequestNodeException(state.Current, typeof(HttpRequestVm));
        Body = httpRequest.Body ?? string.Empty;

        _state.CurrentHasChanged += OnSelectedRequestHasChanged;
    }

    private void OnSelectedRequestHasChanged(object? e, IRequestVm node)
    {
        if (node is not HttpRequestVm httpRequest) throw new InvalidRequestNodeException(node, typeof(HttpRequestVm));
        Body = httpRequest.Body ?? string.Empty;
    }

    partial void OnBodyChanged(string? value)
    {
        if (_state.Current is null) return;
        if (_state.Current is not HttpRequestVm httpRequest) throw new InvalidRequestNodeException(_state.Current, typeof(HttpRequestVm));

        httpRequest.Body = value ?? string.Empty;
        _repository.Update(httpRequest);
    }
}
