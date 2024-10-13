using System;
using Owl.Enums;

namespace Owl.Models.Requests;

public class RequestBase : IRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public RequestAuth? Auth { get; set; }
}
