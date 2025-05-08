using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Models;
using Owl.Models.Requests;
using Owl.Repositories.RequestNode;

namespace Owl.ViewModels.Components;

public partial class CreateIRequestItemDropDownViewModel : ViewModelBase
{
    private readonly IRequestNodeRepository _requestNodeRepository;
    [ObservableProperty] private bool _isPopupOpen = false;

    public CreateIRequestItemDropDownViewModel(IRequestNodeRepository requestNodeRepository)
    {
        _requestNodeRepository = requestNodeRepository;
    }

    [RelayCommand]
    private void SetIsPopupOpen()
    {
        IsPopupOpen = !IsPopupOpen;
    }

    [RelayCommand]
    private void AddDirectory()
    {
        var newNode = new GroupRequest
        {
            Name = "New Directory",
        };

        _requestNodeRepository.Add(newNode);
        IsPopupOpen = false;
    }

    [RelayCommand]
    private void AddHttpRequest()
    {
        var newNode = new HttpRequest
        {
            Name = "New Request",
        };

        _requestNodeRepository.Add(newNode);
        IsPopupOpen = false;
    }
}
