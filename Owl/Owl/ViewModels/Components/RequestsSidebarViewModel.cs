using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.States;

namespace Owl.ViewModels.Components;

public partial class RequestsSidebarViewModel : ViewModelBase
{
    [ObservableProperty] private string _search = string.Empty;
    [ObservableProperty] private ObservableCollection<RequestNode> _requests;
    [ObservableProperty] private ISelectedNodeState _state;

    private readonly IRequestNodeRepository _repository;

    public RequestsSidebarViewModel(ISelectedNodeState state, IRequestNodeRepository repository)
    {
        _state = state;
        _repository = repository;

        _requests = new ObservableCollection<RequestNode>(repository.GetAll());
        // TODO: Remove this, it's just for a test purposes
        // _requests.FirstOrDefault().Children = [new RequestNode{ Name = "Child!" }];
    }

    [RelayCommand]
    private void RefreshData(System.Guid requestNodeId)
    {
        State.Current = _repository.Get(requestNodeId);
    }

    [RelayCommand]
    private void RemoveRequest(RequestNode node)
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
            Method = "GET",
            Body = string.Empty,
        };

        Requests.Add(_repository.Add(newNode));
    }

    [RelayCommand]
    private void RemoveRequestNode(RequestNode node)
    {
        _repository.Delete(node.Id);
        Requests.Remove(node);
    }

    partial void OnSearchChanging(string value)
    {
        Requests = new ObservableCollection<RequestNode>(_repository.Find((x) => x.Name.Contains(value)));
    }
}
