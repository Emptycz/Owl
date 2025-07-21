using Avalonia.Controls;
using Owl.ViewModels;

namespace Owl.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}
