using Avalonia.Controls;
using Avalonia.Interactivity;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.ViewModels.RequestTabs;

namespace Owl.Views.RequestTabs;

public partial class ParamsTab : UserControl
{
    public ParamsTab(IRequestNodeRepository repo)
    {
        InitializeComponent();
        DataContext = new ParamsTabViewModel(repo);
    }

    private void OnRemoveParameterClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button menuItem || menuItem.DataContext is not RequestParameter nodeToRemove) return;

        // Access the RequestViewModel instance from DataContext of UserControl
        if (DataContext is ParamsTabViewModel viewModel)
        {
            viewModel.RemoveParameterCommand.Execute(nodeToRemove);
        }
    }

    private void ParamHasChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox) return;
        if (DataContext is not ParamsTabViewModel viewModel) return;

        viewModel.ParamHasChangedCommand.Execute(null);
    }
}
