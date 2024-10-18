using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Owl.Interfaces;
using Owl.Repositories.RequestNode;
using Owl.ViewModels.Models;
using Serilog;

namespace Owl.Views.Requests.ContextMenus;

public partial class IRequestItemContextMenu : UserControl
{
    public static readonly StyledProperty<IRequestVm> RequestProperty =
        AvaloniaProperty.Register<IRequestItem, IRequestVm>(nameof(Request),
            defaultBindingMode: BindingMode.TwoWay);

    public IRequestVm Request
    {
        get => GetValue(RequestProperty);
        set => SetValue(RequestProperty, value);
    }

    private IRequestNodeRepository _requestNodeRepository;

    public IRequestItemContextMenu()
    {
        InitializeComponent();
    }

    public void Setup(IRequestNodeRepository nodeRepository)
    {
        _requestNodeRepository = nodeRepository;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        Log.Information("TEST!!!!!!!!!!!!!!!!!!!!!!§");
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
            HttpRequestVm requestVm => new HttpRequestItemContextMenu(_requestNodeRepository) { Request = requestVm },
            // GroupRequestVm requestVm => new GroupRequestItemContextMenu { DataContext = requestVm },
            _ => throw new NotImplementedException($"The request type: {Request.GetType()} is not supported.")
        };
    }
}
