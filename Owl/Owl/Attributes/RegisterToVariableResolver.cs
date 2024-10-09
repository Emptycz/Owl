using System;
using Owl.Models.Variables;
using Owl.Services.VariableResolvers;

namespace Owl.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterVariableResolver(IVariableResolver resolver) : Attribute
{
    public Type VariableType { get; set; } = type;
}
