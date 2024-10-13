using Avalonia.Controls;
using Owl.Interfaces;
using Owl.Repositories.RequestNode;
using Owl.ViewModels;

namespace Owl.Views;

public partial class RequestNodeFormWindow : Window
{
    public RequestNodeFormWindow(IRequestNodeRepository repository, IRequestVm httpRequest)
    {
        InitializeComponent();
        DataContext = new RequestNodeFormWindowViewModel(repository, httpRequest);
    }
}
