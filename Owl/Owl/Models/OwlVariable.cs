using System;

namespace Owl.Models;

public interface IVariable
{
    Guid Id { get; init; }
    string Key { get; set; }
    // This contains the config object in JSON format
}

public class OwlVariable : IVariable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
