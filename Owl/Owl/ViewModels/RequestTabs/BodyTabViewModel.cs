using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Models;
using Owl.States;

namespace Owl.ViewModels.RequestTabs;

public partial class BodyTabViewModel(ISelectedNodeState? state) : ViewModelBase
{
    [ObservableProperty] private ISelectedNodeState? _state = state;
}