using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;

namespace Owl.Views.TemplatedControls;

public class ImageButton : TemplatedControl
{
	public static readonly StyledProperty<Bitmap?> IconSourceProperty =
		AvaloniaProperty.Register<ImageButton, Bitmap?>(nameof(IconSource));

	public static readonly StyledProperty<string> IconTitleProperty =
		AvaloniaProperty.Register<ImageButton, string>(nameof(IconTitle), string.Empty);

	public Bitmap? IconSource
	{
		get => GetValue(IconSourceProperty);
		set => SetValue(IconSourceProperty, value);
	}

	public string IconTitle
	{
		get => GetValue(IconTitleProperty);
		set => SetValue(IconTitleProperty, value);
	}
}
