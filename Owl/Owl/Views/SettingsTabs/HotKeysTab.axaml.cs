using Avalonia.Controls;
using Owl.Repositories.Settings;
using Owl.ViewModels.SettingsTabs;

namespace Owl.Views.SettingsTabs;

public partial class HotKeysTab : UserControl
{
    public HotKeysTab(ISettingsRepository settingsRepository)
    {
        InitializeComponent();
        DataContext = new HotKeysTabViewModel(settingsRepository);
    }
}
