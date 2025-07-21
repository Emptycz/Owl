using Avalonia.Controls;
using Owl.ViewModels.SettingsTabs;

namespace Owl.Views.SettingsTabs;

public partial class HotKeysTab : UserControl
{
    public HotKeysTab()
    {
        InitializeComponent();
        DataContext = new HotKeysTabViewModel();
    }
}
