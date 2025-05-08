using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Interfaces;
using Owl.Repositories.RequestNode;

namespace Owl.ViewModels;

public partial class RequestNodeFormWindowViewModel : ViewModelBase
{
    [ObservableProperty] private IRequestVm _current;

    private readonly IRequestNodeRepository _repository;

    public RequestNodeFormWindowViewModel(IRequestNodeRepository repository, IRequestVm request)
    {
        _current = request;
        _repository = repository;
    }

    [RelayCommand]
    private void UpdateSubmit()
    {
        _repository.Update(Current.ToRequest());
    }
}
