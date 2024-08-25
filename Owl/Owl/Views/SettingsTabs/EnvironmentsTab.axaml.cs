using Avalonia.Controls;
using Owl.Repositories.Environment;
using Owl.ViewModels.SettingsTabs;

namespace Owl.Views.SettingsTabs;

public partial class EnvironmentsTab : UserControl
{
    public EnvironmentsTab(IEnvironmentRepository environmentRepository)
    {
        InitializeComponent();
        DataContext = new EnvironmentsTabsViewModel(environmentRepository);
    }
}
