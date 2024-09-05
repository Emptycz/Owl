using System;
using System.Collections.Generic;
using Owl.Models.Variables;

namespace Owl.Models;

public class Environment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public IEnumerable<IVariable> Variables { get; set; } = [];
}
