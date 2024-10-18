using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Owl.Repositories.RequestNode;
using Owl.ViewModels.Models;

namespace Owl.Views.Requests.ContextMenus;

public partial class HttpRequestItemContextMenu : UserControl
{
    public static readonly StyledProperty<HttpRequestVm> RequestProperty =
        AvaloniaProperty.Register<IRequestItem, HttpRequestVm>(nameof(Request),
            defaultBindingMode: BindingMode.TwoWay);

    private readonly IRequestNodeRepository _nodeRepository;

    public HttpRequestVm Request
    {
        get => GetValue(RequestProperty);
        set => SetValue(RequestProperty, value);
    }

    public HttpRequestItemContextMenu(IRequestNodeRepository nodeRepository)
    {
        InitializeComponent();

        _nodeRepository = nodeRepository;
    }

    private void OnRemoveMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem) return;
        _nodeRepository.Remove(Request.Id);
    }

    private void OnDuplicateMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem) return;
        _nodeRepository.Add(Request.ToRequest());
    }

    private void OpenRequestEditWindow(object? sender, RoutedEventArgs e)
    {
        // if (viewModel.State.Current is null) return;
        //
        // var requestEditWindow = new RequestNodeFormWindow(_nodeRepository, viewModel.State.Current);
        // requestEditWindow.ShowDialog(this.FindAncestorOfType<Window>()!);
    }
}
