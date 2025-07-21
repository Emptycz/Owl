using Avalonia.Controls;
using Avalonia.Input;
using Owl.ViewModels;

namespace Owl.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }

    private void CloseSpotlightWindow(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel) return;
        viewModel.SpotlightViewModel.CloseCommand.Execute(null);
    }
}
