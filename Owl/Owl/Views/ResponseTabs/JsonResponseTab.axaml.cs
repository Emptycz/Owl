using System;
using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Indentation.CSharp;
using Owl.States;
using Owl.ViewModels.ResponseTabs;

namespace Owl.Views.ResponseTabs;

public partial class JsonResponseTab : UserControl
{
    public JsonResponseTab(IRequestNodeState state)
    {
        InitializeComponent();
        DataContext = new JsonResponseTabViewModel(state);
        InitializeResponseEditor();
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
}
