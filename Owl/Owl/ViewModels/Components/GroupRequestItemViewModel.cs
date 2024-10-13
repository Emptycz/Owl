using Owl.States;

namespace Owl.ViewModels.Components;

public class GroupRequestItemViewModel : ViewModelBase
{

    private readonly IRequestNodeState _state;
    public GroupRequestItemViewModel()
    {
        _state = RequestNodeState.Instance;
    }
}
