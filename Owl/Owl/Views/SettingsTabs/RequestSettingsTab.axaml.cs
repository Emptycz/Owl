using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Owl.Repositories.Settings;
using Owl.ViewModels.SettingsTabs;

namespace Owl.Views.SettingsTabs;

public partial class RequestSettingsTab : UserControl
{
    public RequestSettingsTab(ISettingsRepository settingsRepository)
    {
        InitializeComponent();
        InitFontComboBox();
        DataContext = new RequestSettingsTabViewModel(settingsRepository);
    }

    private void InitFontComboBox()
    {
        ComboBox? comboBox = this.Find<ComboBox>("FontComboBox");
        if (comboBox is null)
        {
            return;
        }

        comboBox.ItemsSource = FontManager.Current.SystemFonts.OrderBy(x => x.Name);
    }
}
