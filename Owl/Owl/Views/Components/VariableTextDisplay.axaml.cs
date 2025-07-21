using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Avalonia;

namespace Owl.Views.Components;

public partial class VariableTextDisplay : UserControl
{
    public static readonly StyledProperty<string> TemplateTextProperty =
        AvaloniaProperty.Register<VariableTextDisplay, string>(
            nameof(TemplateText));

    public static readonly StyledProperty<ICommand> VariableClickCommandProperty =
        AvaloniaProperty.Register<VariableTextDisplay, ICommand>(
            nameof(VariableClickCommand));

    public string TemplateText
    {
        get => GetValue(TemplateTextProperty);
        set
        {
            SetValue(TemplateTextProperty, value);
        }
    }

    public ICommand VariableClickCommand
    {
        get => GetValue(VariableClickCommandProperty);
        set => SetValue(VariableClickCommandProperty, value);
    }

    public VariableTextDisplay()
    {
        InitializeComponent();
    }

    private static void OnTemplateTextChanged(AvaloniaObject sender, bool before)
    {
        if (sender is VariableTextDisplay control)
        {
            control.RenderTemplateText(control.TemplateText);
        }
    }

    private StackPanel? Container => this.FindControl<StackPanel>("PART_Container");

    private void RenderTemplateText(string text)
    {
        Container?.Children.Clear();
        var parts = Regex.Split(text ?? string.Empty, @"({{.*?}})");

        foreach (var part in parts)
        {
            if (Regex.IsMatch(part, @"{{.*?}}"))
            {
                var button = new Button
                {
                    Content = part,
                    Margin = new Thickness(2, 0),
                    Command = VariableClickCommand,
                    CommandParameter = part
                };
                Container?.Children.Add(button);
            }
            else if (!string.IsNullOrEmpty(part))
            {
                Container?.Children.Add(new TextBlock
                {
                    Text = part,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                });
            }
        }
    }
}