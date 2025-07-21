using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Owl.Repositories.Environment;

namespace Owl.ViewModels.SettingsTabs;

public partial class EnvironmentsTabViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Owl.Models.Environment> _environments;
    [ObservableProperty] private Owl.Models.Environment? _selectedEnvironment;

    private readonly IEnvironmentRepository _environmentRepository;
    public EnvironmentsTabViewModel()
    {
        _environmentRepository = App.Current?.Services?.GetRequiredService<IEnvironmentRepository>() ?? throw new InvalidOperationException("Environment repository is not registered in the service collection.");
        _environments = new ObservableCollection<Owl.Models.Environment>(_environmentRepository.GetAll());
    }

    [RelayCommand]
    private void AddVariable()
    {
        if (SelectedEnvironment is null) return;
        // SelectedEnvironment.Variables
    }
}
