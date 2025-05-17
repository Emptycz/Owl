using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Owl.Views.Components;

public partial class IconButton : UserControl
{
	public static readonly StyledProperty<string?> IconSourceProperty =
		AvaloniaProperty.Register<ImageButton, string?>(nameof(IconSource));

	public static readonly StyledProperty<string> IconTitleProperty =
		AvaloniaProperty.Register<ImageButton, string>(nameof(IconTitle), string.Empty);

	public static readonly StyledProperty<ICommand?> CommandProperty =
		AvaloniaProperty.Register<ImageButton, ICommand?>(nameof(Command));

	public static readonly StyledProperty<string?> CommandParameterProperty =
		AvaloniaProperty.Register<ImageButton, string?>(nameof(CommandParameter));

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

	public ICommand? Command
	{
		get => GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	public string? CommandParameter
	{
		get => GetValue(CommandParameterProperty);
		set => SetValue(CommandParameterProperty, value);
	}

	public IconButton()
	{
		InitializeComponent();
	}
}
