using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Indentation.CSharp;
using Owl.Repositories.RequestNodeRepository;
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
        InitializeResponseEditor();

        AddSidebarPanel(state, nodeRepository);
        DataContext = new RequestViewModel(nodeRepository, state);
    }

    private void AddSidebarPanel(ISelectedNodeState state, IRequestNodeRepository nodeRepository)
    {
        var sidebarWrapper = this.FindControl<Panel>("SidebarWrapper");
        sidebarWrapper?.Children.Add(new RequestsSidebar(state, nodeRepository));
    }

    private void InitializeResponseEditor()
    {
        var responseEditor = this.FindControl<TextEditor>("ResponseEditor")!;

        responseEditor.ShowLineNumbers = true;
        responseEditor.Options.HighlightCurrentLine = true;
        responseEditor.Options.EnableHyperlinks = false;
        responseEditor.Options.EnableEmailHyperlinks = false;
        // responseEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        responseEditor.Options.IndentationSize = 4;
        responseEditor.WordWrap = true;
        responseEditor.Options.ConvertTabsToSpaces = true;

        responseEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(responseEditor.Options);
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
