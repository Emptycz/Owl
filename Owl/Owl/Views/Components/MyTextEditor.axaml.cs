using System;
using Avalonia.Controls;
using Avalonia.Input;
using Owl.ViewModels;

namespace Owl.Views.Components;

public partial class MyTextEditor : UserControl
{
    private readonly TextBox? _textBox;
                            
    public MyTextEditor()
    {
        InitializeComponent();
        DataContext = new TextEditorViewModel();
        _textBox = this.FindControl<TextBox>("TextBox");

        _textBox?.AddHandler(KeyDownEvent, TextBox_PreviewKeyDown!, handledEventsToo: true);
    }
    
    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _textBox?.Focus();
    }
    
    private void AttachEventHandlers()
    {
        _textBox?.AddHandler(KeyDownEvent, TextBox_PreviewKeyDown!, handledEventsToo: true);
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Tab) return;
    
        if (sender is TextBox textBox)
        {
            int caretIndex = textBox.CaretIndex;
            textBox.Text = textBox.Text?.Insert(caretIndex, "\t");
            textBox.CaretIndex = caretIndex + 1;
        }
        e.Handled = true; // Prevent the default behavior of changing focus.
    }
}