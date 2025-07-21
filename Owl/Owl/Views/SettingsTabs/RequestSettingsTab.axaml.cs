using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Owl.ViewModels.SettingsTabs;

namespace Owl.Views.SettingsTabs;

public partial class RequestSettingsTab : UserControl
{
    public RequestSettingsTab()
    {
        InitializeComponent();
        InitFontComboBox();
        DataContext = new RequestSettingsTabViewModel();
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
