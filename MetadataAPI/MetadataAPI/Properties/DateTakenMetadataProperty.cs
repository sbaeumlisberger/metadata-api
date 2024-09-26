using System;
using System.Collections.Generic;
using System.Globalization;
using WIC;

namespace MetadataAPI.Properties;

public class DateTakenMetadataProperty : MetadataPropertyBase<DateTime?>
{
    public static DateTakenMetadataProperty Instance { get; } = new DateTakenMetadataProperty();

    public override string Identifier => nameof(DateTakenMetadataProperty);

    public override IReadOnlySet<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff, ContainerFormat.Png, ContainerFormat.Heif };

    private DateTakenMetadataProperty() { }

    public override DateTime? Read(IMetadataReader metadataReader)
    {
        if (metadataReader.CodecInfo.GetContainerFormat() == ContainerFormat.Heif)
        {
            if (metadataReader.GetMetadata("/ifd/{ushort=306}") is string ifd306)
            {
                return DateTime.ParseExact(ifd306, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            }
            return null;
        }
        else
        {
            return (DateTime?)metadataReader.GetMetadata("System.Photo.DateTaken");
        }
    }

    public override void Write(IMetadataWriter metadataWriter, DateTime? value)
    {
        //if (FileExtensions.Heif.Contains(metadataWriter.FileType))
        //{
        //    if (value is DateTime dateTaken)
        //    {
        //        string formatted = dateTaken.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
        //        metadataWriter.SetMetadata("/ifd/{ushort=306}", formatted);
        //    }
        //    else
        //    {
        //        metadataWriter.SetMetadata("/ifd/{ushort=306}", null);
        //    }
        //}
        //else
        //{
        metadataWriter.SetMetadata("System.Photo.DateTaken", value);
        //}
    }

}
