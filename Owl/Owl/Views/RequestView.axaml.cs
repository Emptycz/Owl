using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using Owl.Models;
using Owl.Repositories.RequestNodeRepository;
using Owl.ViewModels;
using TextMateSharp.Grammars;

namespace Owl.Views;

public partial class RequestView : UserControl
{
    private TextEditor? _editor;
    private RegistryOptions? _registryOptions;
    private IRequestNodeRepository _nodeRepository;

    public RequestView(IRequestNodeRepository nodeRepository)
    {
        InitializeComponent();
        InitializeEditor();
        InitializeResponseEditor();
        DataContext = new RequestViewModel(nodeRepository);
        _nodeRepository = nodeRepository;
    }

    private void InitializeEditor()
    {
        _editor = this.FindControl<TextEditor>("Editor")!;

        _editor.ShowLineNumbers = true;
        _editor.Options.HighlightCurrentLine = true;
        _editor.Options.EnableHyperlinks = false;
        // _editor.Options.ColumnRulerPositions = new List<int>() { 80, 100 };
        _editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(_editor.Options);
        _registryOptions = new RegistryOptions(ThemeName.Dark);

        // TODO: Define JSON structure and highlight rules
        var highlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        _editor.SyntaxHighlighting = highlighting;

        _editor.InstallTextMate(_registryOptions);
        _editor.TextArea.TextEntering += TextEditor_TextEntering!;
    }

    private void InitializeResponseEditor()
    {
        var responseEditor = this.FindControl<TextEditor>("ResponseEditor")!;

        responseEditor.ShowLineNumbers = true;
        responseEditor.Options.HighlightCurrentLine = true;
        responseEditor.Options.EnableHyperlinks = true;
        responseEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        responseEditor.Options.IndentationSize = 4;

        responseEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(responseEditor.Options);
    }

    private void TextEditor_TextEntering(object sender, TextInputEventArgs e)
    {
        if (sender is not TextEditor editor) return;
        if (e.Text != Environment.NewLine) return;

        var caret = editor.TextArea.Caret;
        var line = editor.Document.GetLineByNumber(caret.Line);
        string? lineText = editor.Document.GetText(line.Offset, line.Length);
        
        // Find the leading whitespace of the current line
        string leadingWhitespace = new string(lineText.TakeWhile(char.IsWhiteSpace).ToArray());

        // Insert the leading whitespace at the new line
        editor.Document.Insert(caret.Offset, leadingWhitespace);
        editor.TextArea.Caret.Offset += leadingWhitespace.Length;
    }

    private void Editor_TextChanged(object sender, EventArgs e)
    {
        if (DataContext is not RequestViewModel viewModel || _editor == null) return;
        // Update ViewModel from TextEditor content
        viewModel.SetBodyCommand.Execute(Editor.Text);
    }

    private void OnRemoveMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is not RequestNode nodeToRemove) return;

        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is RequestViewModel viewModel)
        {
            viewModel.RemoveRequestCommand.Execute(nodeToRemove);
        }
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

    private void OpenRequestEditWindow(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not RequestViewModel viewModel) return;

        if (viewModel.SelectedRequest is null) return;

        var requestEditWindow = new RequestNodeFormWindow(_nodeRepository, viewModel.SelectedRequest);
        requestEditWindow.ShowDialog(this.FindAncestorOfType<Window>()!);
    }

    private void RemoveHeaderButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is not string headerKey) return;
        if (DataContext is not RequestViewModel viewModel) return;

        // viewModel.RemoveHeaderCommand.Execute(headerKey);
    }

    private void TabHeaders_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine("SDADSADSADasd");
        throw new NotImplementedException();
    }
}
