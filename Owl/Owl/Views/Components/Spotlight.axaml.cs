using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Threading;
using Owl.ViewModels.Components;

namespace Owl.Views.Components;

public partial class Spotlight : UserControl
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Spotlight, bool>(nameof(IsOpen), defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay);

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    private ItemsControl? SpotlightList => this.FindControl<ItemsControl>("SpotlightListBox");

    public Spotlight()
    {
        InitializeComponent();

        KeyDown += SpotlightBox_OnKeyDown;
        KeyDown += SpotlightBox_OnEnterDown;
        DataContextChanged += OnDataContextChanged;

        Bind(IsVisibleProperty, this.GetObservable(IsOpenProperty));
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not SpotlightViewModel viewModel) return;

        viewModel.IsOpenChanged += (_, isOpen) =>
        {
            if (!isOpen) return;
            // Use Dispatcher to ensure focus is set after rendering
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                SpotlightBox.Focus(); // Set focus to the SpotlightBox
            });
        };
    }

    private void SpotlightBox_OnEnterDown(object? sender, KeyEventArgs e)
    {
        if (SpotlightList is null) return;
        if (e.Key != Key.Enter || SpotlightList.ItemCount <= 0) return;
        if (DataContext is not SpotlightViewModel viewModel) return;
        viewModel.UseSelectedItemCommand.Execute(null);
    }

    private void SpotlightBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (SpotlightList is null) return;
        if (e.Key != Key.Down || sender is not TextBox || SpotlightList.ItemCount <= 0) return;

        bool listBoxFocused = SpotlightList.Focus();
        if (!listBoxFocused) return;

        // Find the first ListBoxItem and focus it
        var firstItem = SpotlightList.ContainerFromIndex(0);
        if (firstItem is ListBoxItem listBoxItem) listBoxItem.Focus();
    }

    private void ItemElementOnPointerPressed(object? sender, TappedEventArgs e)
    {
        if (sender is not ListBox) return;
        if (DataContext is not SpotlightViewModel viewModel) return;

        viewModel.UseSelectedItemCommand.Execute(null);
    }
}
