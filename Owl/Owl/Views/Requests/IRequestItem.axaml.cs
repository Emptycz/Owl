using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Owl.Interfaces;
using Owl.ViewModels.Components;
using Owl.ViewModels.Models;

namespace Owl.Views.Requests;

public partial class IRequestItem : UserControl
{
    public static readonly StyledProperty<IRequestVm> RequestProperty =
        AvaloniaProperty.Register<IRequestItem, IRequestVm>(nameof(Request),
            defaultBindingMode: BindingMode.TwoWay);

    public IRequestVm Request
    {
        get => GetValue(RequestProperty);
        set => SetValue(RequestProperty, value);
    }

    public IRequestItem()
    {
        InitializeComponent();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (Request is null)
        {
            throw new NullReferenceException($"{nameof(Request)} property cannot be null");
        }

        var control = this.FindControl<ContentControl>(nameof(ContentControl));
        if (control is null)
        {
            throw new NullReferenceException($"{nameof(ContentControl)} cannot be null");
        }

        // TODO: We could utilize source gen for this
        control.Content = Request switch
        {
            HttpRequestVm requestVm => new HttpRequestItem { DataContext = requestVm },
            GroupRequestVm requestVm => new GroupRequestItem { DataContext = new GroupRequestItemViewModel(requestVm) },
            _ => throw new NotImplementedException($"The request type: {Request.GetType()} is not supported.")
        };
    }
}
