using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

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

    public Spotlight()
    {
        InitializeComponent();
        Focusable = true;
        Bind(IsVisibleProperty, this.GetObservable(IsOpenProperty));
    }
}
