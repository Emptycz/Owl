using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Owl.Views;

public partial class MainWindow : Window
{
    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Resolve RequestView from the DI container
        var requestView = (RequestView)serviceProvider.GetRequiredService(typeof(RequestView));

        // Assuming you have a ContentControl or similar in your MainWindow.axaml to host RequestView
        var contentHost = this.FindControl<ContentControl>("RequestViewHost");
        if (contentHost != null)
        {
            contentHost.Content = requestView;
        }
    }
}