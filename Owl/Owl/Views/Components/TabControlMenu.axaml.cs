
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Owl.AvaloniaObjects;
using Owl.ViewModels;

namespace Owl.Views.Components;

public partial class TabControlMenu : UserControl
{
    public static readonly StyledProperty<ObservableCollection<TabControlMenuItem>> MenuItemsProperty =
        AvaloniaProperty.Register<TabControlMenu, ObservableCollection<TabControlMenuItem>>(nameof(MenuItems));

    public ObservableCollection<TabControlMenuItem> MenuItems
    {
        get => GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    public TabControlMenu()
    {
        Console.WriteLine("test 0");
        InitializeComponent();
        Console.WriteLine("test 1");
        if (MenuItems == null)
        {
            Console.WriteLine("test 2");
            MenuItems = new ObservableCollection<TabControlMenuItem>();
        }
        Console.WriteLine("test 3");
        DataContext = new TabControlMenuViewModel() { MenuItems = new ObservableCollection<TabControlItem>(MenuItems.Select(mn => new TabControlItem(mn.Text))) };
    }
    
    private void SelectItemClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("test");
        if (sender is not MenuItem menuItem || menuItem.DataContext is not TabControlItem item) return;

        Console.WriteLine("test 2");
        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is TabControlMenuViewModel viewModel)
        {
            Console.WriteLine("test 3");
            viewModel.SelectItemCommand.Execute(item);
        }
    }
}
