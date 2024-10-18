using System;
using Avalonia.Controls;
using Owl.ViewModels.Components;
using Serilog;

namespace Owl.Views.Components;

public partial class CreateIRequestItemDropDown : UserControl
{
    public CreateIRequestItemDropDown()
    {
        InitializeComponent();
    }

    private void DropdownPopup_OnClosed(object? sender, EventArgs e)
    {
        if (DataContext is not CreateIRequestItemDropDownViewModel context)
        {
            Log.Error("DataContext is not created from CreateIRequestItemDropDownViewModel");
            return;
        }
        context.IsPopupOpen = false;
    }
}
