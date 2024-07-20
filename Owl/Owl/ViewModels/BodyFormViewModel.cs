using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Owl.ViewModels;

public partial class BodyFormViewModel : ViewModelBase
{
    [ObservableProperty] private string _body = string.Empty;
    
    partial void OnBodyChanged(string value)
    {
        Console.WriteLine("Body changed: {0}", value);
    }
}