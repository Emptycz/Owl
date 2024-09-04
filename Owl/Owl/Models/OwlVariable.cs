using System;

namespace Owl.Models;

public abstract class VariableBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
}

public class OwlVariable : VariableBase
{
    // public Guid Id { get; init; } = Guid.NewGuid();
    // public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public OwlVariable()
    {
        
    }
}

public class DynamicVariable : VariableBase
{
    public Guid RequestNodeId { get; set; }
    public string ParsingPath { get; set; } = string.Empty;
}
