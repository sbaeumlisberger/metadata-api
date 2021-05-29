using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIC;

namespace MetadataAPI.Properties
{
    public class AuthorMetadataProperty : MetadataPropertyBase<string[]>
    {
        public static AuthorMetadataProperty Instance { get; } = new AuthorMetadataProperty();

        public override string Identifier => nameof(AuthorMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff, ContainerFormat.Heif };

        private AuthorMetadataProperty() { }

        public override string[] Read(IMetadataReader metadataReader)
        {
            return (string[]?)metadataReader.GetMetadata("System.Author") ?? Array.Empty<string>();
        }

        public override void Write(IMetadataWriter metadataWriter, string[]? value)
        {
            if (metadataWriter.CodecInfo.GetContainerFormat() == ContainerFormat.Heif)
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
