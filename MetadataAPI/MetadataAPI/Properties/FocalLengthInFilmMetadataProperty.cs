using System;
using System.Collections.Generic;
using WIC;

namespace MetadataAPI.Properties;

public class FocalLengthInFilmMetadataProperty : MetadataPropertyBase<ushort?>
{
    public static FocalLengthInFilmMetadataProperty Instance { get; } = new FocalLengthInFilmMetadataProperty();

    public override string Identifier { get; } = nameof(FocalLengthInFilmMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

    private FocalLengthInFilmMetadataProperty() { }

    public override ushort? Read(IMetadataReader metadataReader)
    {
        return (ushort?)metadataReader.GetMetadata("System.Photo.FocalLengthInFilm");
    }

    public override void Write(IMetadataWriter metadataWriter, ushort? value)
    {
        metadataWriter.SetMetadata("System.Photo.FocalLengthInFilm", value);
    }

}
