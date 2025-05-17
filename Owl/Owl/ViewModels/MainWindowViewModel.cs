using System;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Spotlight;
using Owl.ViewModels.Components;

namespace Owl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public SpotlightViewModel SpotlightViewModel { get; }

    public MainWindowViewModel(IServiceProvider provider)
    {
        SpotlightViewModel = new SpotlightViewModel(
            provider.GetRequiredService<ISpotlightRepository>(),
            provider.GetRequiredService<IRequestNodeRepository>()
        );
    }

    [RelayCommand]
    private void ToggleSpotlight()
    {
        SpotlightViewModel.IsOpen = !SpotlightViewModel.IsOpen;
    }
}
