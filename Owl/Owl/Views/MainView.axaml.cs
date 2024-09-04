using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Spotlight;
using Owl.States;
using Owl.ViewModels;

namespace Owl.Views;

public partial class MainView : UserControl
{
    public MainView(IServiceProvider provider)
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel(provider);
    }
}
