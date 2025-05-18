using Avalonia.Controls;
using Owl.ViewModels;

namespace Owl.Views;

public partial class RequestView : UserControl
{

    public RequestView()
    {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not RequestViewModel viewModel) return;

        viewModel.SelectedRequestPropertyHasChangedCommand.Execute(null);
    }

    private void TextBox_UrlChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox) return;
        if (DataContext is not RequestViewModel viewModel) return;

        viewModel.SelectedRequestPropertyHasChangedCommand.Execute(null);
    }
}
