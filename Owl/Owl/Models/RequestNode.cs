using System;
using System.Collections.Generic;
using System.Net.Http;
using LiteDB;

namespace Owl.Models;

public class RequestNode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string Method { get; set; } = "GET";
    public string? Body { get; set; }
    public List<RequestHeader> Headers { get; set; } = [];
    public List<RequestParameter> Parameters { get; set; } = [];
    public RequestAuth? Auth { get; set; }
    public IEnumerable<RequestNode> Children { get; set; } = new List<RequestNode>();

    [BsonIgnore]
    public HttpResponseMessage? Response { get; set; }
}
