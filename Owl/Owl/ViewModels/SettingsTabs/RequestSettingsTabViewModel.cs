using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Owl.Models;
using Owl.Repositories.Settings;

namespace Owl.ViewModels.SettingsTabs;

public partial class RequestSettingsTabViewModel : ViewModelBase
{
    [ObservableProperty] private string _fontFamily = "Arial";
    [ObservableProperty] private int _fontSize = 12;
    [ObservableProperty] private bool _showLineNumbers = true;
    [ObservableProperty] private Settings _settings;

    [ObservableProperty] private bool _showHorizontalBar = true;
    [ObservableProperty] private bool _showVerticalBar = true;
    [ObservableProperty] private int _indentationSize = 4;
    [ObservableProperty] private bool _useTabs = true;

    private readonly ISettingsRepository _settingsRepository;

    public RequestSettingsTabViewModel()
    {
        Console.WriteLine("Init RequestSettingsTabViewModel");
        _settingsRepository = App.Current?.Services?.GetRequiredService<ISettingsRepository>() ?? throw new InvalidOperationException("Settings repository is not registered in the service collection.");
        Console.WriteLine("2.");
        _settingsRepository.RepositoryHasChanged += (_, settings) => Settings = settings.NewValue;
        Console.WriteLine("3.");
        Settings = _settingsRepository.Current;

        if (_settingsRepository?.Current is null) Console.WriteLine("tet");

        Console.WriteLine(_settingsRepository?.Current?.Id);
        Console.WriteLine(Settings.Id);
        Console.WriteLine(Settings.RequestSettings.ShowLineNumbers);
        Console.WriteLine(Settings.RequestSettings.FontFamily);
        Console.WriteLine(_settingsRepository?.GetAll().Count());
    }

    partial void OnSettingsChanged(Settings value)
    {
        _settingsRepository.Update(value);
    }
}
