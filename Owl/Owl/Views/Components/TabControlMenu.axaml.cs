using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Owl.AvaloniaObjects;
using Owl.ViewModels;

namespace Owl.Views.Components;

public partial class TabControlMenu : UserControl
{
    public static readonly DirectProperty<TabControlMenu, IEnumerable<TabControlMenuItem>> ItemsProperty =
        AvaloniaProperty.RegisterDirect<TabControlMenu, IEnumerable<TabControlMenuItem>>(nameof(Items), o => o._items, (o, v) => o._items = v);

    private IEnumerable<TabControlMenuItem> _items = new AvaloniaList<TabControlMenuItem>();
    public IEnumerable<TabControlMenuItem> Items
    {
        get => _items;
        set => UpdateItems(value);
    }

    private void UpdateItems(IEnumerable<TabControlMenuItem> value)
    {
        _items = value;
        RaisePropertyChanged(ItemsProperty, null, value);
    }

    public TabControlMenu()
    {
        InitializeComponent();
        DataContext = new TabControlMenuViewModel() { MenuItems = new ObservableCollection<TabControlItem>(Items.Select(mn => new TabControlItem(mn.Text))) };
    }

    private void SelectItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is not TabControlItem item) return;

        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is TabControlMenuViewModel viewModel)
        {
            viewModel.SelectItemCommand.Execute(item);
        }
    }
}
