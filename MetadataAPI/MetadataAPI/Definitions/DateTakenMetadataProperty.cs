using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI.Definitions
{
    public class DateTakenMetadataProperty : IMetadataProperty<DateTime?>
    {
        public static DateTakenMetadataProperty Instance { get; } = new DateTakenMetadataProperty();

        public string Identifier => nameof(DateTakenMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes => new HashSet<string>(
            FileExtensions.Jpeg
            .Concat(FileExtensions.Tiff)
            .Concat(FileExtensions.Heif)
            .Concat(FileExtensions.Png));

        private DateTakenMetadataProperty() { }

        public DateTime? Read(IReadMetadata metadataReader)
        {
            if (FileExtensions.Heif.Contains(metadataReader.FileType))
            {
                if ((string)metadataReader.GetMetadata("/ifd/{ushort=306}") is string ifd306)
                {
                    return DateTime.ParseExact(ifd306, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                }
                return null;
                // ?? metadataReader.GetMetadata("/xmp/xmp:CreateDate")
                // ?? metadataReader.GetMetadata("/xmp/exif:DateTimeOriginal")
            }
            else
            {
                return (DateTime?)metadataReader.GetMetadata("System.Photo.DateTaken");
            }
        }

        public void Write(IWriteMetadata metadataWriter, DateTime? value)
        {
            if (FileExtensions.Heif.Contains(metadataWriter.FileType))
            {
                if (value is DateTime dateTaken)
                {
                    string formatted = dateTaken.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                    metadataWriter.SetMetadata("/ifd/{ushort=306}", formatted);
                }
                else
                {
                    metadataWriter.SetMetadata("/ifd/{ushort=306}", null);
                }
            }
            else
            {
                metadataWriter.SetMetadata("System.Photo.DateTaken", value);
            }
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (DateTime?)value);
        }
    }
}
