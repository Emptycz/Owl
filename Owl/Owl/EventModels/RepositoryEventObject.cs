using Owl.Enums;

namespace Owl.EventModels;

public record RepositoryEventObject<T>
{
    public RepositoryEventOperation Operation { get; init; }
    public T? NewValue { get; init; }

    public RepositoryEventObject(T newValue, RepositoryEventOperation operation)
    {
        Operation = operation;
        NewValue = newValue;
    }

    public RepositoryEventObject(RepositoryEventOperation operation)
    {
        Operation = operation;
    }
}
