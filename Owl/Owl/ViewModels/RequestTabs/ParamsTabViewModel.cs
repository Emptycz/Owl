using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Owl.Exceptions;
using Owl.Interfaces;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.States;
using Owl.ViewModels.Models;

namespace Owl.ViewModels.RequestTabs;

public partial class ParamsTabViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<RequestParameter> _parameters;
    [ObservableProperty] private HttpRequestVm _request;

    private readonly IRequestNodeState _requestState;
    private readonly IRequestNodeRepository _repository;

    public ParamsTabViewModel(IRequestNodeRepository repo)
    {
        _repository = repo;
        _requestState = RequestNodeState.Instance;

        if (_requestState.Current is not HttpRequestVm httpRequest) throw new InvalidRequestNodeException(_requestState.Current, typeof(HttpRequestVm));
        _request = httpRequest;

        _parameters = new ObservableCollection<RequestParameter>(_request.Parameters);
        _requestState.CurrentHasChanged += OnSelectedRequestHasChanged;

        // TODO: Move this to the model itself and create DataModels for the DB entities
        foreach (var param in _parameters)
        {
            param.PropertyChanged += OnParameterChanged;
        }
    }

    private void OnSelectedRequestHasChanged(object? e, IRequestVm node)
    {
        if (node is not HttpRequestVm vm)
        {
            Console.WriteLine("Request has changed and it's not HttpRequestVm");
            Reset();
            return;
        }
        Console.WriteLine($"Request has changed and it's HttpRequestVm, reading Id: {node.Id}");
        Parameters = new ObservableCollection<RequestParameter>(vm.Parameters);
    }

    // TODO: Move this to the model itself and create DataModels for the DB entities
    private void OnParameterChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RequestParameter.IsEnabled))
        {
            ParamHasChanged();
        }
    }

    [RelayCommand]
    private void SetParamEnabled(string value)
    {
        foreach (var param in Parameters)
        {
            param.IsEnabled = value == "true";
        }
    }

    [RelayCommand]
    private void ParamHasChanged()
    {
        if (_requestState.Current is not HttpRequestVm httpRequest) return;
        httpRequest.Parameters = Parameters.ToList();
        _repository.Update(httpRequest.ToRequest());
    }

    [RelayCommand]
    private void AddParameter()
    {
        if (_requestState.Current is null) return;
        Parameters.Add(new RequestParameter());
        ParamHasChanged();
    }

    [RelayCommand]
    private void RemoveParameter(RequestParameter parameter)
    {
        if (_requestState.Current is null) return;
        Parameters.Remove(parameter);
        ParamHasChanged();
    }

    private void Reset()
    {
        Parameters = [];
    }
}
