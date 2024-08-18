using Avalonia.Controls;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.ViewModels;

namespace Owl.Views;

public partial class RequestNodeFormWindow : Window
{
    public RequestNodeFormWindow(IRequestNodeRepository repository, RequestNode requestNode)
    {
        InitializeComponent();
        DataContext = new RequestNodeFormWindowViewModel(repository, requestNode);
    }
}