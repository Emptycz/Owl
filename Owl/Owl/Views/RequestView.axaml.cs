using System;
using Avalonia.Controls;
using AvaloniaEdit;
using Microsoft.Extensions.DependencyInjection;
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

    private readonly IServiceProvider _serviceProvider;

    public RequestView(IServiceProvider provider)
    {
        InitializeComponent();
        _serviceProvider = provider;

        AddSidebarPanel();
        DataContext = new RequestViewModel(
            provider.GetRequiredService<IRequestNodeRepository>(),
            provider.GetRequiredService<ISelectedNodeState>()
        );      
    }

    private void AddSidebarPanel()
    {
        var sidebarWrapper = this.FindControl<Panel>("SidebarWrapper");
        sidebarWrapper?.Children.Add(new RequestsSidebar(_serviceProvider));
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
