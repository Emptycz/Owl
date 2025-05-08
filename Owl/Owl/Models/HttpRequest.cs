using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using LiteDB;
using Owl.Enums;
using Owl.Models.Requests;
using Owl.Services;
using Owl.ViewModels.Models;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace Owl.Models;

public class HttpRequest : RequestBase
{
    public string? Url { get; set; }
    public HttpRequestMethod Method { get; set; } = HttpRequestMethod.Get;
    // TODO: We will need to support more types of Body, such as FormData, GraphQL body, etc. This will need to be structured, not just a string
    public string? Body { get; set; }
    public IList<RequestHeader> Headers { get; set; } = [];
    public IList<RequestParameter> Parameters { get; set; } = [];

    [BsonIgnore]
    public HttpResponseMessage? Response { get; set; }


    public HttpRequest Clone()
    {
        return new HttpRequest
        {
            Id = Id,
            Name = Name,
            Url = Url,
            Method = Method,
            Body = Body,
            Headers = Headers.Select(h => new RequestHeader { Key = h.Key, Value = h.Value }).ToList(),
            Parameters = Parameters.Select(p => new RequestParameter { Key = p.Key, Value = p.Value }).ToList(),
            Auth = Auth?.Clone(),
            Response = Response
        };
    }

    public HttpRequest()
    {

    }

    public HttpRequest(HttpRequestVm vm)
    {
        Id = vm.Id;
        Name = vm.Name;
        Url = vm.Url;
        Method = vm.Method;
        Body = vm.Body;
        Headers = vm.Headers;
        Parameters = vm.Parameters;
        Auth = vm.Auth;
        Response = vm.Response;
    }

    public void ReplaceVariable(FoundVariable foundVariable, string resolvedVariableValue)
    {
        switch (foundVariable.Location)
        {
            case VariableLocation.Url:
                Url = Url?.Replace($"{{{{ .{foundVariable.Key} }}}}", resolvedVariableValue);
                break;
            case VariableLocation.Body:
                Body = Body?.Replace($"{{{{ .{foundVariable.Key} }}}}", resolvedVariableValue);
                break;
            case VariableLocation.Header:
                Headers = Headers.Select(header =>
                {
                    header.Value = header.Value.Replace($"{{{{ .{foundVariable.Key} }}}}", resolvedVariableValue);
                    return header;
                }).ToList();
                break;
            case VariableLocation.Parameter:
                Parameters = Parameters.Select(param =>
                {
                    param.Value = param.Value.Replace($"{{{{ .{foundVariable.Key} }}}}", resolvedVariableValue);
                    return param;
                }).ToList();
                break;
            case VariableLocation.Auth:
                Auth!.Token =
                    Auth.Token.Replace($"{{{{ .{foundVariable.Key} }}}}", resolvedVariableValue);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Location: {foundVariable.Location} not supported");
        }
    }
}
