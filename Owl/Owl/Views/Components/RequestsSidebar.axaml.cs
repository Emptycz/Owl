using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Owl.Interfaces;
using Owl.ViewModels.Components;
using Owl.Views.Windows;
using Serilog;

namespace Owl.Views.Components;

public partial class RequestsSidebar : UserControl
{
    public RequestsSidebar()
    {
        InitializeComponent();
    }

    private void OnRemoveMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is not IRequestVm nodeToRemove) return;

        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is RequestsSidebarViewModel viewModel)
        {
            viewModel.RemoveRequestCommand.Execute(nodeToRemove);
        }
    }

    private void OnDuplicateMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is not IRequestVm node) return;

        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is RequestsSidebarViewModel viewModel)
        {
            viewModel.DuplicateRequestCommand.Execute(node);
        }
    }
}
