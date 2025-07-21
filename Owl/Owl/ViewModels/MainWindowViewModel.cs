using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.ViewModels.Components;
using Owl.Views;
using Owl.Views.Pages;
using Serilog;

namespace Owl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public SpotlightViewModel SpotlightViewModel { get; }

    [ObservableProperty] private UserControl _currentView;

    public MainWindowViewModel()
    {
        SpotlightViewModel = new SpotlightViewModel();
        _currentView = new HomePageView();
    }

    [RelayCommand]
    private void Route(string? route)
    {
        if (string.IsNullOrEmpty(route)) return;

        // TODO: We should to init and pass ViewModels instead of Views
        switch (route.ToLower())
        {
            case "home":
                Log.Debug("Navigating to home");
                CurrentView = new HomePageView();
                break;
            case "request":
                Log.Debug("Navigating to request");
                CurrentView = new RequestView();
                break;
            case "settings":
                Log.Debug("Navigating to settings");
                CurrentView = new SettingsPageView();
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
