using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;

namespace Owl.ViewModels;

public partial class BodyFormViewModel : ViewModelBase
{
    [ObservableProperty] private string _body = string.Empty;

    partial void OnBodyChanged(string value)
    {
        Log.Debug("Body changed: {0}", value);
    }
}
