using System.Threading;
using CommunityToolkit.Mvvm.Input;

namespace Owl.ViewModels.ResponseTabs;

public partial class ProcessingResponseTabViewModel : ViewModelBase
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ProcessingResponseTabViewModel(CancellationTokenSource cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
    }

    [RelayCommand]
    private void CancelRequest()
    {
        _cancellationTokenSource.Cancel();
    }
}
