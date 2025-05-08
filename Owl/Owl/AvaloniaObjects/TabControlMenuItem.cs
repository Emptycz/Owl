using Avalonia;

namespace Owl.AvaloniaObjects;

public class TabControlMenuItem : AvaloniaObject
{
	private static readonly StyledProperty<string> TextProperty =
		AvaloniaProperty.Register<TabControlMenuItem, string>(nameof(Text));
	
	private static readonly StyledProperty<string> KeyProperty =
		AvaloniaProperty.Register<TabControlMenuItem, string>(nameof(Key));
	
	public string Text
	{
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}
	
	public string Key
	{
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}
}
