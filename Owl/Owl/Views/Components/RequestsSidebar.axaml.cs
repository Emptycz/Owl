using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Owl.Models;
using Owl.Repositories.RequestNodeRepository;
using Owl.States;
using Owl.ViewModels.Components;

namespace Owl.Views.Components;

public partial class RequestsSidebar : UserControl
{
    private readonly IRequestNodeRepository _nodeRepository;
    
    public RequestsSidebar(ISelectedNodeState state, IRequestNodeRepository nodeRepository)
    {
        InitializeComponent();
        DataContext = new RequestsSidebarViewModel(state, nodeRepository);
        _nodeRepository = nodeRepository;
    }

    private void OpenRequestEditWindow(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not RequestsSidebarViewModel viewModel) return;

        if (viewModel.State.Current is null) return;

        var requestEditWindow = new RequestNodeFormWindow(_nodeRepository, viewModel.State.Current);
        requestEditWindow.ShowDialog(this.FindAncestorOfType<Window>()!);
    }

    private void OnRemoveMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is not RequestNode nodeToRemove) return;

        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is RequestsSidebarViewModel viewModel)
        {
            viewModel.RemoveRequestCommand.Execute(nodeToRemove);
        }
    }
}