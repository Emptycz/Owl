using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;

namespace Owl.Views.Components;

public partial class IconButton : UserControl
{
	public static readonly StyledProperty<string?> IconSourceProperty =
		AvaloniaProperty.Register<ImageButton, string?>(nameof(IconSource));

	public static readonly StyledProperty<string> IconTitleProperty =
		AvaloniaProperty.Register<ImageButton, string>(nameof(IconTitle), string.Empty);

	public string? IconSource
	{
		get => GetValue(IconSourceProperty);
		set => SetValue(IconSourceProperty, value);
	}

	public string IconTitle
	{
		get => GetValue(IconTitleProperty);
		set => SetValue(IconTitleProperty, value);
	}

	public IconButton()
	{
		InitializeComponent();
	}
}
