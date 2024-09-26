using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.States;

namespace Owl.ViewModels.RequestTabs;

public partial class ParamsTabViewModel : ViewModelBase
{
    [ObservableProperty] private IRequestNodeState _requestState;
    [ObservableProperty] private ObservableCollection<RequestParameter> _parameters;

    private readonly IRequestNodeRepository _repository;

    public ParamsTabViewModel(IRequestNodeState state, IRequestNodeRepository repo)
    {
        _repository = repo;
        _requestState = state;
        _parameters = new ObservableCollection<RequestParameter>(RequestState.Current?.Parameters ?? []);
        _requestState.CurrentHasChanged += OnSelectedRequestHasChanged;

        // TODO: Move this to the model itself and create DataModels for the DB entities
        foreach (var param in _parameters)
        {
            param.PropertyChanged += OnParameterChanged;
        }
    }

    private void OnSelectedRequestHasChanged(object? e, RequestNode node)
    {
        Parameters = new ObservableCollection<RequestParameter>(node.Parameters);
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
        if (RequestState.Current == null) return;

        Console.WriteLine("Params have changed! Current params: {0}", JsonConvert.SerializeObject(Parameters));
        RequestState.Current.Parameters = Parameters.ToList();
        _repository.Update(RequestState.Current);
    }

    [RelayCommand]
    private void AddParameter()
    {
        if (RequestState.Current is null) return;
        Parameters.Add(new RequestParameter());
        ParamHasChanged();
    }

    [RelayCommand]
    private void RemoveParameter(RequestParameter parameter)
    {
        if (RequestState.Current is null) return;
        Parameters.Remove(parameter);
        ParamHasChanged();
    }
}
