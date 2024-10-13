using System;
using Owl.Interfaces;
using Owl.Models.Requests;

namespace Owl.Exceptions;

public class InvalidRequestNodeException : Exception
{
    public InvalidRequestNodeException(IRequest request, Type expectedType)
        : base($"Invalid IRequest, expected: {expectedType}, recieved: {request}")
    {
    }

    public InvalidRequestNodeException(IRequestVm request, Type expectedType)
        : base($"Invalid IRequestVm, expected: {expectedType}, recieved: {request}")
    {
    }
}
