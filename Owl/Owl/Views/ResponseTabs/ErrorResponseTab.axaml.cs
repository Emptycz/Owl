using Avalonia.Controls;
using Owl.States;
using Owl.ViewModels.ResponseTabs;

namespace Owl.Views.ResponseTabs;

public partial class ErrorResponseTab : UserControl
{
    public ErrorResponseTab(string error)
    {
        InitializeComponent();
        DataContext = new ErrorResponseTabViewModel(error);
    }
}
