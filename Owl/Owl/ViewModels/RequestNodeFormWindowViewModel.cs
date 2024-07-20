using System;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Contexts;
using Owl.Models;
using Owl.Repositories.RequestNodeRepository;

namespace Owl.ViewModels;

public partial class RequestNodeFormWindowViewModel : ViewModelBase
{
    [ObservableProperty] private RequestNode _currentNode;

    private readonly IRequestNodeRepository _repository;
    
    public RequestNodeFormWindowViewModel(IRequestNodeRepository repository, RequestNode requestNode)
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