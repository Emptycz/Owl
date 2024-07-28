using System;
using Avalonia.Controls;

namespace Owl.Views;

public partial class MainView : UserControl
{
    public MainView()
    { 
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine("Selection changed");
    }
}
