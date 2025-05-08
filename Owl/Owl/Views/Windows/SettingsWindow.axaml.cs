using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Owl.Repositories.Environment;
using Owl.Repositories.Settings;
using Owl.ViewModels.Windows;

namespace Owl.Views.Windows;

public partial class SettingsWindow : Window
{
    public SettingsWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        DataContext = new SettingsWindowViewModel(
            serviceProvider.GetRequiredService<IEnvironmentRepository>(),
            serviceProvider.GetRequiredService<ISettingsRepository>()
        );
    }
}
