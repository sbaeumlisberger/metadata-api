using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;
using WIC;

namespace MetadataAPI.Properties
{
    public class OrientationMetadataProperty : MetadataPropertyBase<PhotoOrientation>
    {
        public static OrientationMetadataProperty Instance { get; } = new OrientationMetadataProperty();

        public override string Identifier { get; } = nameof(OrientationMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private OrientationMetadataProperty() { }

        public override PhotoOrientation Read(IMetadataReader metadataReader)
        {
            UInt16? orientation = (UInt16?)metadataReader.GetMetadata("System.Photo.Orientation");
            return orientation != null ? (PhotoOrientation)orientation : PhotoOrientation.Unspecified;
        }

        public override void Write(IMetadataWriter metadataWriter, PhotoOrientation value)
        {
            if (value == PhotoOrientation.Unspecified)
            {
                metadataWriter.SetMetadata("System.Photo.Orientation", null);
            }
            else
            {
                metadataWriter.SetMetadata("System.Photo.Orientation", (UInt16)value);
            }
        }

    }
}
