using Avalonia.Controls;
using Owl.States;
using Owl.ViewModels.ResponseTabs;

namespace Owl.Views.ResponseTabs;

public partial class RawResponseTab : UserControl
{
    public RawResponseTab()
    {
        InitializeComponent();
        DataContext = new RawResponseTabViewModel();
    }
}
