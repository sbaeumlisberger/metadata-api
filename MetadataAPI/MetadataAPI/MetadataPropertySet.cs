using System.Collections;
using System.Collections.Generic;

namespace MetadataAPI;

public class MetadataPropertySet : IReadOnlyCollection<(IMetadataProperty Property, object? Value)>
{
    public int Count => internalSet.Count;

    private readonly ISet<(IMetadataProperty, object?)> internalSet = new HashSet<(IMetadataProperty, object?)>();

    public void Add<T>(IMetadataProperty<T> property, T value)
    {
        internalSet.Add((property, value));
    }

    public IEnumerator<(IMetadataProperty, object?)> GetEnumerator()
    {
        return internalSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return internalSet.GetEnumerator();
    }
}
