using System;
using System.Collections.Generic;

namespace Owl.Models;

public class RequestGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; init; }
    public List<RequestNode> RequestNodes { get; set; } = [];

    public RequestGroup(string name)
    {
        Name = name;
    }

    public RequestGroup()
    {

    }
}
