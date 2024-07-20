using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using Owl.ViewModels;
using TextMateSharp.Grammars;

namespace Owl.Views.Components;

public partial class BodyFormView : UserControl
{
    public BodyFormView()
    {
        InitializeComponent();
    }
}