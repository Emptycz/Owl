using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Models;
using Owl.Repositories.Settings;

namespace Owl.ViewModels.SettingsTabs;

public partial class HotKeysTabViewModel : ViewModelBase
{
    [ObservableProperty] private IEnumerable<HotKey> _hotKeys;

    private readonly ISettingsRepository _settingsRepository;

    public HotKeysTabViewModel(ISettingsRepository settingsRepository)
    {
        HotKeys = settingsRepository.Current.HotKeysSettings;

        _settingsRepository = settingsRepository;
        _settingsRepository.RepositoryHasChanged += (_, settings) => HotKeys = settings.HotKeysSettings;
    }
}
