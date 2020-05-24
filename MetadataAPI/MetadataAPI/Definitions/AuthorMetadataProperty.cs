using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI.Definitions
{
    public class AuthorMetadataProperty : IMetadataProperty<string[]>
    {
        public static AuthorMetadataProperty Instance { get; } = new AuthorMetadataProperty();

        public string Identifier => nameof(AuthorMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes => new HashSet<string>() { ".jpe", ".jpeg", ".jpg", ".tiff", ".tif", ".heic" };

        private AuthorMetadataProperty() { }

        public string[] Read(IReadMetadata metadataReader)
        {
            if (metadataReader.FileType == ".heic")
            {
                return (string[])(
                   //metadataReader.GetMetadata("ifd/{ushort=315}")
                   //?? 
                   metadataReader.GetMetadata("/xmp/dc:creator")
                   //?? metadataReader.GetMetadata("ifd/{ushort=40093}")
                   ?? metadataReader.GetMetadata("/xmp/tiff:artist"));
            }
            else
            {
                return (string[])metadataReader.GetMetadata("System.Author") ?? new string[0];
            }
        }

        public void Write(IWriteMetadata metadataWriter, string[] value)
        {
            if (metadataWriter.FileType == ".heic")
            {
                metadataWriter.SetMetadata("/ifd/{ushort=315}", "");
                metadataWriter.SetMetadata("/xmp/tiff:artist", "Author1");
                metadataWriter.SetMetadata("/ifd/{ushort=40093}", "");
                metadataWriter.SetMetadata("/xmp/tiff:artist", value);
            }
            else
            {
                metadataWriter.SetMetadata("System.Author", value);
            }
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (string[])value);
        }
    }
}
