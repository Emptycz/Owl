using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Attributes;
using Owl.Repositories.Environment;
using Owl.Repositories.Settings;
using Owl.Views.SettingsTabs;

namespace Owl.ViewModels.Windows;

public partial class SettingsWindowViewModel : ViewModelBase
{
    [ObservableProperty] private UserControl? _content;
    [ObservableProperty] private SettingTab _selectedTab = SettingTab.General;

    private readonly IEnvironmentRepository _environmentRepository;
    private readonly ISettingsRepository _settingsRepository;
    public SettingTab[] SettingTabs { get; } = [
        SettingTab.General,
        SettingTab.Environments,
        SettingTab.Hotkeys,
        SettingTab.Request,
        SettingTab.Response,
        SettingTab.Themes,
    ];

    public SettingsWindowViewModel(IEnvironmentRepository environmentRepository, ISettingsRepository settingsRepository)
    {
        _environmentRepository = environmentRepository;
        _settingsRepository = settingsRepository;
    }

    partial void OnSelectedTabChanged(SettingTab value)
    {
        Content = value switch
        {
            SettingTab.Environments => new EnvironmentsTab(_environmentRepository),
            SettingTab.Hotkeys => new HotKeysTab(_settingsRepository),
            SettingTab.Request => new RequestSettingsTab(_settingsRepository),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}

public enum SettingTab
{
    [DisplayName("General")]
    General,
    [DisplayName("Hotkeys")]
    Hotkeys,
    [DisplayName("Environments")]
    Environments,
    [DisplayName("Request")]
    Request,
    [DisplayName("Response")]
    Response,
    [DisplayName("Themes")]
    Themes,
}
