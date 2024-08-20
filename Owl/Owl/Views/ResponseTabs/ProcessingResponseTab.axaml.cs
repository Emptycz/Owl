using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Owl.ViewModels.ResponseTabs;

namespace Owl.Views.ResponseTabs;

public partial class ProcessingResponseTab : UserControl
{
    public ProcessingResponseTab(CancellationTokenSource cancellationTokenSource)
    {
        InitializeComponent();
        DataContext = new ProcessingResponseTabViewModel(cancellationTokenSource);
    }
}
