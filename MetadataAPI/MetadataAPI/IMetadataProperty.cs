namespace MetadataAPI;

public interface IMetadataProperty : IReadonlyMetadataProperty
{
    void Write(IMetadataWriter metadataWriter, object? value);
}

public interface IMetadataProperty<T> : IMetadataProperty, IReadonlyMetadataProperty<T>
{
    void Write(IMetadataWriter metadataWriter, T value);
}
