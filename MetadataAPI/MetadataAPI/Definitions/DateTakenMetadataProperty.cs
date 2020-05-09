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
        public string Identifier => nameof(DateTakenMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes => new HashSet<string>(
            FileTypes.JpegExtensions
            .Concat(FileTypes.TiffExtensions)
            .Concat(FileTypes.HeifExtensions)
            .Concat(FileTypes.PngExtensions));

        public DateTime? Read(IMetadataReader metadataReader)
        {
            if (FileTypes.HeifExtensions.Contains(metadataReader.FileType))
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

        public void Write(IMetadataWriter metadataWriter, DateTime? value)
        {
            if (FileTypes.HeifExtensions.Contains(metadataWriter.FileType))
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
    }
}
