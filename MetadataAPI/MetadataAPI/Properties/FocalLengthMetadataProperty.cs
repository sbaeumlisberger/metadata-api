using System;
using System.Collections.Generic;
using WIC;

namespace MetadataAPI.Properties;

public class FocalLengthMetadataProperty : MetadataPropertyBase<double?>
{
    public static FocalLengthMetadataProperty Instance { get; } = new FocalLengthMetadataProperty();

    public override string Identifier { get; } = nameof(FocalLengthMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

    private FocalLengthMetadataProperty() { }

    public override double? Read(IMetadataReader metadataReader)
    {
        return (double?)metadataReader.GetMetadata("System.Photo.FocalLength");
    }

    public override void Write(IMetadataWriter metadataWriter, double? value)
    {
        metadataWriter.SetMetadata("System.Photo.FocalLength", value);
    }
}
