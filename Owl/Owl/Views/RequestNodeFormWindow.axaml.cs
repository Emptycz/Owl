using Avalonia.Controls;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.ViewModels;
using Owl.ViewModels.Models;

namespace Owl.Views;

public partial class RequestNodeFormWindow : Window
{
    public RequestNodeFormWindow(IRequestNodeRepository repository, RequestNodeVm requestNode)
    {
        InitializeComponent();
        DataContext = new RequestNodeFormWindowViewModel(repository, requestNode);
    }
}
