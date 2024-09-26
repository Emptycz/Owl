using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Owl.Enums;
using Owl.Models;
using Owl.Repositories.Environment;
using Owl.Repositories.RequestNode;
using Owl.Services;
using Owl.States;
using Owl.ViewModels.Models;
using Environment = Owl.Models.Environment;

namespace Owl.ViewModels.Components;

public partial class RequestsSidebarViewModel : ViewModelBase
{
    [ObservableProperty] private string _search = string.Empty;
    [ObservableProperty] private ObservableCollection<RequestNodeVm> _requests;
    [ObservableProperty] private IRequestNodeState _state;

    [ObservableProperty] private ObservableCollection<Environment> _environments;
    [ObservableProperty] private Environment? _selectedEnvironment;

    private readonly IRequestNodeRepository _repository;
    private readonly IEnvironmentRepository _environmentRepository;

    public RequestsSidebarViewModel(IServiceProvider provider)
    {
        _environmentRepository = provider.GetRequiredService<IEnvironmentRepository>();
        var vars = _environmentRepository.GetAll().ToList();
        if (vars.Count == 0)
        {
            vars.Add(_environmentRepository.Add(new Environment { Name = "Default" }));
        }

        _environments = new ObservableCollection<Environment>(vars);
        _selectedEnvironment = _environments.FirstOrDefault();

        _state = provider.GetRequiredService<IRequestNodeState>();
        _repository = provider.GetRequiredService<IRequestNodeRepository>();
        _requests = new ObservableCollection<RequestNodeVm>(_repository.GetAll().Select(r => new RequestNodeVm(r)));

        // TODO: Remove this, it's just for a test purposes
        // _requests.FirstOrDefault().Children = [new RequestNode{ Name = "Child!" }];
    }

    [RelayCommand]
    private void RefreshData(Guid requestNodeId)
    {
        var request = _repository.Get(requestNodeId);
        State.Current = request is null ? null : new RequestNodeVm(request);
    }

    [RelayCommand]
    private void RemoveRequest(RequestNodeVm node)
    {
        _repository.Delete(node.Id);
        Requests.Remove(node);
    }

    [RelayCommand]
    private void AddNode()
    {
        var newNode = new RequestNode
        {
            Name = "New Request",
            Method = HttpRequestType.Get,
            Body = string.Empty,
        };

        Requests.Add(new RequestNodeVm(_repository.Add(newNode)));
    }

    [RelayCommand]
    private void RemoveRequestNode(RequestNodeVm node)
    {
        _repository.Delete(node.Id);
        Requests.Remove(node);
    }

    [RelayCommand]
    private void DuplicateRequest(RequestNodeVm node)
    {
        var newNode = new RequestNode
        {
            Name = node.Name,
            Method = node.Method,
            Body = node.Body,
            Url = node.Url,
            Headers = node.Headers,
            Children = node.Children,
        };
        Requests.Add(new RequestNodeVm(_repository.Add(newNode)));
    }

    partial void OnSelectedEnvironmentChanged(Environment? oldValue, Environment? newValue)
    {
        if (newValue is null) return;
        if (oldValue == newValue) return;

        var vars = _environmentRepository.Get(newValue.Id)?.Variables;
        if (vars is null) return;
        VariablesManager.AddVariables(vars, newValue?.Name);
    }

    partial void OnSearchChanging(string value)
    {
        Requests = new ObservableCollection<RequestNodeVm>(_repository.Find((x) => x.Name.Contains(value))
            .Select(r => new RequestNodeVm(r)));
    }
}
