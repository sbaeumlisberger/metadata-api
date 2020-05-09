using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI.Definitions
{
    public class AuthorMetadataProperty : IMetadataProperty<string[]>
    {
        public string Identifier => nameof(AuthorMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes => new HashSet<string>() { ".jpe", ".jpeg", ".jpg", ".tiff", ".tif", ".heic" };

        public string[] Read(IMetadataReader metadataReader)
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

        public void Write(IMetadataWriter metadataWriter, string[] value)
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
    }
}
