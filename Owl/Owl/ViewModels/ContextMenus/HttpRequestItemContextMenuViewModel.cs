using CommunityToolkit.Mvvm.Input;
using Owl.Models;
using Owl.Repositories.RequestNode;

namespace Owl.ViewModels.ContextMenus;

public partial class HttpRequestItemContextMenuViewModel : ViewModelBase
{
    private readonly IRequestNodeRepository _nodeRepository;

    public HttpRequestItemContextMenuViewModel(IRequestNodeRepository nodeRepository)
    {
        _nodeRepository = nodeRepository;
    }

    [RelayCommand]
    private void DuplicateHttpNode(HttpRequest node)
    {
        _nodeRepository.Add(node);
    }

    [RelayCommand]
    private void RemoveHttpNode(HttpRequest node)
    {
        _nodeRepository.Remove(node.Id);
    }
}
