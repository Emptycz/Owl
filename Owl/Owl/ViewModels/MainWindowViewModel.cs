using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Spotlight;
using Owl.ViewModels.Components;
using Owl.Views;
using Owl.Views.Pages;
using Serilog;

namespace Owl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public SpotlightViewModel SpotlightViewModel { get; }

    private readonly IServiceProvider _serviceProvider;
    [ObservableProperty] private UserControl _currentView;

    public MainWindowViewModel(IServiceProvider provider)
    {
        SpotlightViewModel = new SpotlightViewModel(
            provider.GetRequiredService<ISpotlightRepository>(),
            provider.GetRequiredService<IRequestNodeRepository>()
        );

        _serviceProvider = provider;
        _currentView = (RequestView)provider.GetRequiredService(typeof(RequestView));
    }

    [RelayCommand]
    private void Route(string? route)
    {
        if (string.IsNullOrEmpty(route)) return;

        switch (route.ToLower())
        {
            case "home":
                Log.Debug("Navigating to home");
                CurrentView = new HomePageView();
                break;
            case "request":
                Log.Debug("Navigating to request");
                CurrentView = new RequestView(_serviceProvider);
                break;
            default:
                throw new NotImplementedException($"Route '{route}' is not implemented.");
        }
    }

    [RelayCommand]
    private void ToggleSpotlight()
    {
        SpotlightViewModel.IsOpen = !SpotlightViewModel.IsOpen;
    }
}
