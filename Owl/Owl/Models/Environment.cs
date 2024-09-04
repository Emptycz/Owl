using System;
using System.Collections.Generic;

namespace Owl.Models;

public class Environment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public IEnumerable<OwlVariable> Variables { get; set; } = [];
}
