using Avalonia.Controls;
using Owl.ViewModels.SettingsTabs;

namespace Owl.Views.SettingsTabs;

public partial class EnvironmentsTab : UserControl
{
    public EnvironmentsTab()
    {
        InitializeComponent();
        DataContext = new EnvironmentsTabViewModel();
    }
}
