using Avalonia.Controls;
using Owl.Repositories.RequestNode;
using Owl.States;
using Owl.ViewModels.RequestTabs;

namespace Owl.Views.RequestTabs;

public partial class AuthTab : UserControl
{
    public AuthTab(IRequestNodeRepository repo)
    {
        InitializeComponent();
        DataContext = new AuthTabViewModel(repo);
    }
}
