using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Owl.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private object? _item;

    partial void OnItemChanged(object? value)
    {
        Console.WriteLine(value);
    }
}
