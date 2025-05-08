using System;
using Owl.Interfaces;
using Owl.Models;
using Owl.Models.Requests;
using Owl.ViewModels.Models;

namespace Owl.Factories;

public static class RequestNodeVmFactory
{
    public static IRequestVm GetRequestNodeVm(IRequest request)
    {
        // TODO: Might use source gen to automate this
        return request switch
        {
            HttpRequest req => new HttpRequestVm(req),
            GroupRequest req => new GroupRequestVm(req),
            _ => throw new NotImplementedException($"Request type {request.GetType().FullName} factory mapper is not implemented."),
        };
    }
}
