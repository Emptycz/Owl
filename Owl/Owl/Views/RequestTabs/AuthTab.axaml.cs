using Avalonia.Controls;
using Owl.Repositories.RequestNodeRepository;
using Owl.States;
using Owl.ViewModels.RequestTabs;

namespace Owl.Views.RequestTabs;

public partial class AuthTab : UserControl
{
    public AuthTab(ISelectedNodeState state, IRequestNodeRepository repo)
    {
        InitializeComponent();
        DataContext = new AuthTabViewModel(state, repo);
    }
}
