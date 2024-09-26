namespace MetadataAPI;

public interface IMetadataWriter : IMetadataReader
{
    void SetMetadata(string name, object? value);
}
