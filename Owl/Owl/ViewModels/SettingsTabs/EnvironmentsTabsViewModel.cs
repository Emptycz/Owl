using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Models;
using Owl.Repositories.Environment;

namespace Owl.ViewModels.SettingsTabs;

public partial class EnvironmentsTabsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Environment> _environments;
    [ObservableProperty] private Environment? _selectedEnvironment;

    private readonly IEnvironmentRepository _environmentRepository;
    public EnvironmentsTabsViewModel(IEnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
        _environments = new ObservableCollection<Environment>(_environmentRepository.GetAll());
    }
}
