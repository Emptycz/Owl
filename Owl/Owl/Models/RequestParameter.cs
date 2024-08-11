using CommunityToolkit.Mvvm.ComponentModel;

namespace Owl.Models;

public partial class RequestParameter : ObservableObject
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    [ObservableProperty] private bool _isEnabled = true;
}