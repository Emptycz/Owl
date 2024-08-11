using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using Owl.Models;
using Owl.States;
using Owl.ViewModels.RequestTabs;
using TextMateSharp.Grammars;

namespace Owl.Views.RequestTabs;

public partial class BodyTab : UserControl
{
    private TextEditor? _editor;
    private RegistryOptions? _registryOptions;
    
    public BodyTab(ISelectedNodeState selectedNodeState)
    {
        DataContext = new BodyTabViewModel(selectedNodeState);
        InitializeComponent();
        InitializeEditor();
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
}