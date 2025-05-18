using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Spotlight;
using Owl.Services.VariableResolvers;
using Owl.States;
using Owl.ViewModels.Components;
using Owl.ViewModels.Pages;
using Serilog;

namespace Owl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public SpotlightViewModel SpotlightViewModel { get; }

    private readonly IServiceProvider _serviceProvider;
    [ObservableProperty] private ViewModelBase _currentView;

    public MainWindowViewModel(IServiceProvider provider)
    {
        SpotlightViewModel = new SpotlightViewModel(
            provider.GetRequiredService<ISpotlightRepository>(),
            provider.GetRequiredService<IRequestNodeRepository>()
        );

        _serviceProvider = provider;
        _currentView = new HomePageViewModel();
    }

    [RelayCommand]
    private void Route(string? route)
    {
        if (string.IsNullOrEmpty(route)) return;

        switch (route.ToLower())
        {
            case "home":
                Log.Debug("Navigating to home");
                CurrentView = new HomePageViewModel();
                break;
            case "request":
                Log.Debug("Navigating to request");
                CurrentView = new RequestViewModel(_serviceProvider);
                break;
            case "collection":
                Log.Debug("Navigating to collection");
                CurrentView = new CollectionViewModel();
                break;
            default:
                throw new NotImplementedException($"Route '{route}' is not implemented.");
        }
    }

    [RelayCommand]
    private void ToggleSpotlight()
    {
        Console.WriteLine("Spotlight toggled");
        Log.Error("Spotlight toggled");
        SpotlightViewModel.IsOpen = !SpotlightViewModel.IsOpen;
    }
}
