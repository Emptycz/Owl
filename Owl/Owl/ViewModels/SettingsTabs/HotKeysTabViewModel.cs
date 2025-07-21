using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Owl.Models;
using Owl.Repositories.Settings;

namespace Owl.ViewModels.SettingsTabs;

public partial class HotKeysTabViewModel : ViewModelBase
{
    [ObservableProperty] private IEnumerable<HotKey> _hotKeys;

    private readonly ISettingsRepository _settingsRepository;

    public HotKeysTabViewModel()
    {
        _settingsRepository = App.Current?.Services?.GetRequiredService<ISettingsRepository>()
          ?? throw new InvalidOperationException("Settings repository is not registered in the service collection.");
        _settingsRepository.RepositoryHasChanged += (_, settings) => HotKeys = settings.NewValue?.HotKeysSettings ?? [];
        HotKeys = _settingsRepository.Current.HotKeysSettings;
    }
}
