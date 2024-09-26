using System;
using System.Collections.Generic;
using System.Linq;
using WIC;

namespace MetadataAPI.Properties;

public class KeywordsMetadataProperty : MetadataPropertyBase<string[]>
{
    public static KeywordsMetadataProperty Instance { get; } = new KeywordsMetadataProperty();

    public override string Identifier { get; } = nameof(KeywordsMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

    private KeywordsMetadataProperty() { }

    public override string[] Read(IMetadataReader metadataReader)
    {
        return (string[])(metadataReader.GetMetadata("System.Keywords") ?? Array.Empty<string>());
    }

    public override void Write(IMetadataWriter metadataWriter, string[] value)
    {
        if (value.Any())
        {
            metadataWriter.SetMetadata("System.Keywords", value.ToArray());
        }
        else
        {
            metadataWriter.SetMetadata("System.Keywords", null);
        }
    }

}
