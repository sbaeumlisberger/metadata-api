using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIC;

namespace MetadataAPI.Properties
{
    public class CameraModelMetadataProperty : MetadataPropertyBase<string>
    {
        public static CameraModelMetadataProperty Instance { get; } = new CameraModelMetadataProperty();

        public override string Identifier { get; } = nameof(CameraModelMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private CameraModelMetadataProperty() { }

        public override string Read(IMetadataReader metadataReader)
        {
            return (string?)metadataReader.GetMetadata("System.Photo.CameraModel") ?? string.Empty;
        }

        public override void Write(IMetadataWriter metadataWriter, string value)
        {
            if (value != string.Empty)
            {
                metadataWriter.SetMetadata("System.Photo.CameraModel", value);
            }
            else
            {
                metadataWriter.SetMetadata("System.Photo.CameraModel", null);
            }
        }
    }
}
