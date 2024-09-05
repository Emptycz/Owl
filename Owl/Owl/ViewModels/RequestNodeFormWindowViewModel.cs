using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Repositories.RequestNode;
using Owl.ViewModels.Models;

namespace Owl.ViewModels;

public partial class RequestNodeFormWindowViewModel : ViewModelBase
{
    [ObservableProperty] private RequestNodeVm _currentNode;

    private readonly IRequestNodeRepository _repository;

    public RequestNodeFormWindowViewModel(IRequestNodeRepository repository, RequestNodeVm requestNode)
    {
        _currentNode = requestNode;
        string currentDirectory = Directory.GetCurrentDirectory();
        _repository = repository;
    }

    [RelayCommand]
    private void UpdateSubmit()
    {
        _repository.Update(CurrentNode);
    }
}
