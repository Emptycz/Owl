using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

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
        Focusable = true;
        KeyDown += SpotlightBox_OnKeyDown;
        Bind(IsVisibleProperty, this.GetObservable(IsOpenProperty));
    }

    private void SpotlightBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (SpotlightList is null) return;
        if (e.Key != Key.Down) return;

        bool res = SpotlightList.Focus();
        Console.WriteLine(res);
    }
}
