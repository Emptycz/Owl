using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Models;
using Owl.Repositories.Environment;

namespace Owl.ViewModels.SettingsTabs;

public partial class EnvironmentsTabViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Environment> _environments;
    [ObservableProperty] private Environment? _selectedEnvironment;

    private readonly IEnvironmentRepository _environmentRepository;
    public EnvironmentsTabViewModel(IEnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
        _environments = new ObservableCollection<Environment>(_environmentRepository.GetAll());
    }

    [RelayCommand]
    private void AddVariable()
    {
        if (SelectedEnvironment is null) return;
        // SelectedEnvironment.Variables
    }
}
