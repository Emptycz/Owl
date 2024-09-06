using System;
using System.Collections.Generic;
using System.Linq;
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


    public RequestNode Clone()
    {
        return new RequestNode
        {
            Id = Id,
            Name = Name,
            Url = Url,
            Method = Method,
            Body = Body,
            Headers = Headers.Select(h => new RequestHeader { Key = h.Key, Value = h.Value }).ToList(),
            Parameters = Parameters.Select(p => new RequestParameter { Key = p.Key, Value = p.Value }).ToList(),
            Auth = Auth?.Clone(),
            Children = Children.Select(c => c.Clone()).ToList(),
            Response = Response
        };
    }
}
