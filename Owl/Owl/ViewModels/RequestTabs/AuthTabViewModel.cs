using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Factories;
using Owl.Interfaces;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.States;

namespace Owl.ViewModels.RequestTabs;

public partial class AuthTabViewModel : ViewModelBase
{
    [ObservableProperty] private IRequestNodeState _requestState;
    [ObservableProperty] private string? _scheme = "Bearer";
    [ObservableProperty] private string? _token = string.Empty;
    public string[] SchemeOptions { get; set; } = ["Bearer"];

    private readonly IRequestNodeRepository _repository;

    public AuthTabViewModel(IRequestNodeState state, IRequestNodeRepository repo)
    {
        _repository = repo;
        _requestState = state;
        _requestState.CurrentHasChanged += OnSelectedRequestHasChanged;

        Scheme = state.Current?.Auth?.Scheme ?? "Bearer";
        Token = state.Current?.Auth?.Token ?? string.Empty;
    }

    private void OnSelectedRequestHasChanged(object? e, IRequestVm node)
    {
        Scheme = node.Auth?.Scheme ?? "Bearer";
        Token = node.Auth?.Token ?? string.Empty;
    }

    partial void OnSchemeChanged(string? value)
    {
        if (RequestState.Current is null) return;
        RequestState.Current.Auth ??= new RequestAuth();

        RequestState.Current.Auth.Scheme = value ?? string.Empty;
        _repository.Update(RequestState.Current.ToRequest());
    }

    partial void OnTokenChanged(string? value)
    {
        if (RequestState.Current is null) return;
        RequestState.Current.Auth ??= new RequestAuth();

        RequestState.Current.Auth.Token = value ?? string.Empty;
        _repository.Update(RequestState.Current.ToRequest());
    }
}
