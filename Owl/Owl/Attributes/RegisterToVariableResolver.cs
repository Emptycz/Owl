using System;

namespace Owl.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterToVariableResolver(Type resolverType) : Attribute
{
    public Type ResolverType { get; set; } = resolverType;
}
