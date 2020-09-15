using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI.Properties
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

        public DateTime? Read(IMetadataReader metadataReader)
        {
            if (FileExtensions.Heif.Contains(metadataReader.FileType))
            {
                if ((string)metadataReader.GetMetadata("/ifd/{ushort=306}") is string ifd306)
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

        public void Write(IMetadataWriter metadataWriter, DateTime? value)
        {
            metadataWriter.SetMetadata("System.Photo.DateTaken", value);            
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object value)
        {
            Write(metadataWriter, (DateTime?)value);
        }
    }
}
