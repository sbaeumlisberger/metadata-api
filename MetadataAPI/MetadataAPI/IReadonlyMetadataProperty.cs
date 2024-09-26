using System;
using System.Collections.Generic;

namespace MetadataAPI;

public interface IReadonlyMetadataProperty
{
    string Identifier { get; }

    IReadOnlySet<Guid> SupportedFormats { get; }

    object? Read(IMetadataReader metadataReader);
}

public interface IReadonlyMetadataProperty<T> : IReadonlyMetadataProperty
{
    new T Read(IMetadataReader metadataReader);
}
