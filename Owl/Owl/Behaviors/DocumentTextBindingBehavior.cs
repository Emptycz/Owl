using System;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.Utils;

namespace Owl.Behaviors;

public class DocumentTextBindingBehavior : Behavior<TextEditor>
{
    private TextEditor? _textEditor = null;

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<DocumentTextBindingBehavior, string>(nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is not { } textEditor) return;

        _textEditor = textEditor;
        _textEditor.TextChanged += TextChanged;
        this.GetObservable(TextProperty).Subscribe(TextPropertyChanged);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (_textEditor != null) _textEditor.TextChanged -= TextChanged;
    }

    private void TextChanged(object? sender, EventArgs eventArgs)
    {
        if (_textEditor?.Document != null)
        {
            Text = _textEditor.Document.Text;
        }
    }

    private void TextPropertyChanged(string? text)
    {
        if (_textEditor?.Document == null) return;
        if (string.IsNullOrEmpty(text)) text = string.Empty;

        int caretOffset = _textEditor.CaretOffset;
        _textEditor.Document.Text = text;
        _textEditor.CaretOffset = caretOffset;
    }
}
