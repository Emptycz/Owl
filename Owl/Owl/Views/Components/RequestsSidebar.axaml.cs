using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Owl.Repositories.RequestNode;
using Owl.ViewModels.Components;
using Owl.ViewModels.Models;
using Owl.Views.Windows;

namespace Owl.Views.Components;

public partial class RequestsSidebar : UserControl
{
    private readonly IRequestNodeRepository _nodeRepository;
    private readonly IServiceProvider _serviceProvider;

    public RequestsSidebar(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        DataContext = new RequestsSidebarViewModel(serviceProvider);
        _serviceProvider = serviceProvider;
        _nodeRepository = serviceProvider.GetRequiredService<IRequestNodeRepository>();
    }

    private void OpenRequestEditWindow(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not RequestsSidebarViewModel viewModel) return;

        if (viewModel.State.Current is null) return;

        var requestEditWindow = new RequestNodeFormWindow(_nodeRepository, viewModel.State.Current);
        requestEditWindow.ShowDialog(this.FindAncestorOfType<Window>()!);
    }

    private void OpenSettingsWindow(object? sender, RoutedEventArgs e)
    {
        var envWindow = new SettingsWindow(_serviceProvider);
        envWindow.ShowDialog(this.FindAncestorOfType<Window>()!);
    }

    private void OnRemoveMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is not RequestNodeVm nodeToRemove) return;

        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is RequestsSidebarViewModel viewModel)
        {
            viewModel.RemoveRequestCommand.Execute(nodeToRemove);
        }
    }

    private void OnDuplicateMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is not RequestNodeVm node) return;

        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is RequestsSidebarViewModel viewModel)
        {
            viewModel.DuplicateRequestCommand.Execute(node);
        }
    }

    private void OpenEnvironmentsWindow(object? sender, RoutedEventArgs e)
    {
        // throw new NotImplementedException();
    }
}
