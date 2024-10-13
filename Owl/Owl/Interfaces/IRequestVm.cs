using System;
using Owl.Enums;
using Owl.Models;
using Owl.Models.Requests;

namespace Owl.Interfaces;

/// <summary>
/// Abstract interface that defines generic structure needed to render in RequestSidebar panel
/// </summary>
public interface IRequestVm
{
    Guid Id { get; set; }
    RequestNodeType Type { get; }
    string Name { get; set; }
    public RequestAuth? Auth { get; set; }

    // TODO: This should either return the XAML element to render or something pre-defined
    string GetIcon();
    IRequest ToRequest();
}
