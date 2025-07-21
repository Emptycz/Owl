using Avalonia.Controls;
using Owl.ViewModels.Windows;

namespace Owl.Views.Pages;

public partial class SettingsPageView : UserControl
{
    public SettingsPageView()
    {
        InitializeComponent();
        DataContext = new SettingsWindowViewModel();
    }
}
