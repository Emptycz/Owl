using CommunityToolkit.Mvvm.ComponentModel;

namespace Owl.ViewModels.ResponseTabs;

public partial class ErrorResponseTabViewModel : ViewModelBase
{
    [ObservableProperty] private string? _response;

    public ErrorResponseTabViewModel(string error)
    {
        Response = error;
    }
}
