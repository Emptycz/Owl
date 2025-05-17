using System;
using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;
using Owl.ViewModels;

namespace Owl.Views;

public partial class MainWindow : Window
{
    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(serviceProvider);

        // // Resolve RequestView from the DI container
        // var requestView = (RequestView)serviceProvider.GetRequiredService(typeof(RequestView));
        //
        // // Assuming you have a ContentControl or similar in your MainWindow.axaml to host RequestView
        // var contentHost = this.FindControl<ContentControl>("RequestViewHost");
        // if (contentHost != null)
        // {
        //     contentHost.Content = requestView;
        // }
    }

    private void CloseSpotlightWindow(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel) return;
        viewModel.SpotlightViewModel.CloseCommand.Execute(null);
    }
}
