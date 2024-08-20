using Avalonia.Controls;
using Owl.ViewModels.ResponseTabs;

namespace Owl.Views.ResponseTabs;

public partial class ErrorResponseTab : UserControl
{
    public ErrorResponseTab(string response)
    {
        InitializeComponent();
        DataContext = new ErrorResponseTabViewModel(response);
    }
}
