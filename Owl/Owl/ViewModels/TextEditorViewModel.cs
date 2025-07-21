using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Owl.ViewModels;

public partial class TextEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<int> _lineNumbers = [1];

    [ObservableProperty]
    private string _textContent = string.Empty;

    [ObservableProperty]
    private int _selectedLineNumber = 1;

    [ObservableProperty]
    private int _caretIndex = 1;

    private RelayCommand UpdateCaretIndexCommand { get; }

    public TextEditorViewModel()
    {
        UpdateCaretIndexCommand = new RelayCommand(UpdateCaretIndex);
    }

    partial void OnSelectedLineNumberChanged(int value)
    {
        UpdateCaretIndexCommand.Execute(value);
    }


    private void UpdateCaretIndex()
    {
        if (SelectedLineNumber < 1 || SelectedLineNumber > TextContent.Split('\n').Length)
        {
            throw new ArgumentOutOfRangeException(nameof(SelectedLineNumber), "Line number is out of range.");
        }

        int charIndex = 0;
        string[] lines = TextContent.Split('\n');

        for (int i = 0; i < SelectedLineNumber - 1; i++)
        {
            charIndex += lines[i].Length + 1;
        }

        CaretIndex = charIndex;
    }

    partial void OnTextContentChanged(string value)
    {
        LineNumbers.Clear();

        string[] lines = TextContent.Split(Environment.NewLine);
        for (int i = 1; i <= lines.Length; i++)
        {
            LineNumbers.Add(i);
        }
    }
}
