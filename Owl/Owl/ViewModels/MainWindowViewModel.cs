using CommunityToolkit.Mvvm.Input;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Spotlight;
using Owl.States;
using Owl.ViewModels.Components;

namespace Owl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public SpotlightViewModel SpotlightViewModel { get; }

    public MainWindowViewModel(ISpotlightRepository repo, IRequestNodeRepository nodeRepo, ISelectedNodeState state)
    {
        SpotlightViewModel = new SpotlightViewModel(repo, nodeRepo, state);
    }

    [RelayCommand]
    private void ToggleSpotlight()
    {
        SpotlightViewModel.IsOpen = !SpotlightViewModel.IsOpen;
    }
}
