using System;

namespace Owl.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MapResolverAttribute(Type resolverType) : Attribute
{
    public Type ResolverType { get; protected set; } = resolverType;
}
