using System.Collections.Generic;

namespace Owl.Models;

public class RequestGroup
{
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
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