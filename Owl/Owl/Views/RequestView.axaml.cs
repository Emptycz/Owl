using Avalonia.Controls;
using Owl.ViewModels;
using Owl.Views.Components;

namespace Owl.Views;

public partial class RequestView : UserControl
{
    public RequestView()
    {
        InitializeComponent();

        AddSidebarPanel();
        DataContext = new RequestViewModel();
    }

    private void AddSidebarPanel()
    {
        var sidebarWrapper = this.FindControl<Panel>("SidebarWrapper");
        sidebarWrapper?.Children.Add(new RequestsSidebar());
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
