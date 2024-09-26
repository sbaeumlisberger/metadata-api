using System;
using System.Collections.Generic;
using WIC;

namespace MetadataAPI.Properties;

public class ISOSpeedMetadataProperty : MetadataPropertyBase<ushort?>
{
    public static ISOSpeedMetadataProperty Instance { get; } = new ISOSpeedMetadataProperty();

    public override string Identifier { get; } = nameof(ISOSpeedMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

    private ISOSpeedMetadataProperty() { }

    public override ushort? Read(IMetadataReader metadataReader)
    {
        return (ushort?)metadataReader.GetMetadata("System.Photo.ISOSpeed");
    }

    public override void Write(IMetadataWriter metadataWriter, ushort? value)
    {
        metadataWriter.SetMetadata("System.Photo.ISOSpeed", value);
    }

}
