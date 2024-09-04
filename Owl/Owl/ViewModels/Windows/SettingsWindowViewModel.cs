using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Repositories.Environment;
using Owl.Repositories.Settings;
using Owl.Views.SettingsTabs;

namespace Owl.ViewModels.Windows;

public partial class SettingsWindowViewModel : ViewModelBase
{
    [ObservableProperty] private UserControl? _content;
    [ObservableProperty] private string _selectedTab = "General";

    private readonly IEnvironmentRepository _environmentRepository;
    private readonly ISettingsRepository _settingsRepository;

    public IEnumerable<string> SettingTabs { get; } =
    [
        "General",
        "Hotkeys",
        "Environments",
        "Request",
        "Response",
        "Themes",
    ];

    public SettingsWindowViewModel(IEnvironmentRepository environmentRepository, ISettingsRepository settingsRepository)
    {
        _environmentRepository = environmentRepository;
        _settingsRepository = settingsRepository;
    }

    partial void OnSelectedTabChanged(string value)
    {
        Content = value switch
        {
            "Environment" => new EnvironmentsTab(_environmentRepository),
            "Hotkeys" => new HotKeysTab(_settingsRepository),
            "Request" => new RequestSettingsTab(_settingsRepository),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
