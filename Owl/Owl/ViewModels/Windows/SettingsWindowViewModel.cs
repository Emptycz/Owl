using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Repositories.Environment;
using Owl.Views.SettingsTabs;

namespace Owl.ViewModels.Windows;

public partial class SettingsWindowViewModel : ViewModelBase
{
    [ObservableProperty] private UserControl? _content;

    public SettingsWindowViewModel(IEnvironmentRepository environmentRepository)
    {
        _content = new EnvironmentsTab(environmentRepository);
    }
}
