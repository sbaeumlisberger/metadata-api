using System;
using System.Collections.Generic;
using WIC;

namespace MetadataAPI.Properties;

public class CopyrightMetadataProperty : MetadataPropertyBase<string>
{
    public static CopyrightMetadataProperty Instance { get; } = new CopyrightMetadataProperty();

    public override string Identifier { get; } = nameof(CopyrightMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

    private CopyrightMetadataProperty() { }

    public override string Read(IMetadataReader metadataReader)
    {
        return (string?)metadataReader.GetMetadata("System.Copyright") ?? string.Empty;
    }

    public override void Write(IMetadataWriter metadataWriter, string value)
    {
        if (value != string.Empty)
        {
            metadataWriter.SetMetadata("System.Copyright", value);
        }
        else
        {
            metadataWriter.SetMetadata("System.Copyright", null);
        }
    }

}
