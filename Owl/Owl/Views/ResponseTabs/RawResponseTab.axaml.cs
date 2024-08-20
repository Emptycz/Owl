using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Owl.States;
using Owl.ViewModels.ResponseTabs;

namespace Owl.Views.ResponseTabs;

public partial class RawResponseTab : UserControl
{
    public RawResponseTab(ISelectedNodeState state)
    {
        InitializeComponent();
        DataContext = new RawResponseTabViewModel(state);
    }
}
