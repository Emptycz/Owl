using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Attributes;
using Owl.Views.SettingsTabs;

namespace Owl.ViewModels.Windows;

public partial class SettingsWindowViewModel : ViewModelBase
{
    [ObservableProperty] private UserControl? _content;
    [ObservableProperty] private SettingTab _selectedTab = SettingTab.General;

    public SettingTab[] SettingTabs { get; } = [
        SettingTab.General,
        SettingTab.Environments,
        SettingTab.Hotkeys,
        SettingTab.Request,
        SettingTab.Response,
        SettingTab.Themes,
    ];

    public SettingsWindowViewModel()
    {
    }

    partial void OnSelectedTabChanged(SettingTab value)
    {
        Content = value switch
        {
            SettingTab.Environments => new EnvironmentsTab(),
            SettingTab.Hotkeys => new HotKeysTab(),
            SettingTab.Request => new RequestSettingsTab(),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}

public enum SettingTab
{
    [Value("General")]
    General,
    [Value("Hotkeys")]
    Hotkeys,
    [Value("Environments")]
    Environments,
    [Value("Request")]
    Request,
    [Value("Response")]
    Response,
    [Value("Themes")]
    Themes,
}
