using Avalonia.Controls;
using AvaloniaEdit;
using Owl.Repositories.RequestNode;
using Owl.States;
using Owl.ViewModels;
using Owl.Views.Components;
using TextMateSharp.Grammars;

namespace Owl.Views;

public partial class RequestView : UserControl
{
    private TextEditor? _editor;
    private RegistryOptions? _registryOptions;

    public RequestView(IRequestNodeRepository nodeRepository, ISelectedNodeState state)
    {
        InitializeComponent();

        AddSidebarPanel(state, nodeRepository);
        DataContext = new RequestViewModel(nodeRepository, state);
    }

    private void AddSidebarPanel(ISelectedNodeState state, IRequestNodeRepository nodeRepository)
    {
        var sidebarWrapper = this.FindControl<Panel>("SidebarWrapper");
        sidebarWrapper?.Children.Add(new RequestsSidebar(state, nodeRepository));
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
