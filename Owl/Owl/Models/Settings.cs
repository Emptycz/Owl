using System;
using System.Collections.Generic;

namespace Owl.Models;

public class Settings
{
    public Guid Id = Guid.NewGuid();
    public RequestSettings RequestSettings { get; set; } = new();
    public IEnumerable<HotKey> HotKeysSettings { get; set; } = [];
}

public class RequestSettings
{
    public string FontFamily { get; set; } = "Arial";
    public int FontSize { get; set; } = 12;
    public bool ShowLineNumbers { get; set; } = true;
}

public class HotKey
{
    public required string Key { get; set; }
    public string? Modifier { get; set; }
    public string? Description { get; set; }
    public string? Action { get; set; }
}
