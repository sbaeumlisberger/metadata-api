using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIC;

namespace MetadataAPI.Properties
{
    public class CameraManufacturerMetadataProperty : MetadataPropertyBase<string>
    {
        public static CameraManufacturerMetadataProperty Instance { get; } = new CameraManufacturerMetadataProperty();

        public override string Identifier { get; } = nameof(CameraManufacturerMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private CameraManufacturerMetadataProperty() { }

        public override string Read(IMetadataReader metadataReader)
        {
            return (string?)metadataReader.GetMetadata("System.Photo.CameraManufacturer") ?? string.Empty;
        }

        public override void Write(IMetadataWriter metadataWriter, string value)
        {
            if (value != string.Empty)
            {
                metadataWriter.SetMetadata("System.Photo.CameraManufacturer", value);
            }
            else 
            {
                metadataWriter.SetMetadata("System.Photo.CameraManufacturer", null);
            }
        }

    }
}
