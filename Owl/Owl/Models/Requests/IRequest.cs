using System;

namespace Owl.Models.Requests;

public interface IRequest
{
    Guid Id { get; set; }
    string Name { get; set; }
    public RequestAuth? Auth { get; set; }
}
