using System;
using System.Collections.Generic;
using WIC;

namespace MetadataAPI.Properties;

public class AuthorMetadataProperty : MetadataPropertyBase<string[]>
{
    public static AuthorMetadataProperty Instance { get; } = new AuthorMetadataProperty();

    public override string Identifier => nameof(AuthorMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff, ContainerFormat.Heif };

    private AuthorMetadataProperty() { }

    public override string[] Read(IMetadataReader metadataReader)
    {
        return (string[]?)metadataReader.GetMetadata("System.Author") ?? Array.Empty<string>();
    }

    public override void Write(IMetadataWriter metadataWriter, string[] value)
    {
        if (value.Length > 0)
        {
            metadataWriter.SetMetadata("System.Author", value);
        }
        else
        {
            metadataWriter.SetMetadata("System.Author", null);
        }
    }

}
