using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIC;

namespace MetadataAPI.Properties
{
    public class FocalLengthMetadataProperty : MetadataPropertyBase<double?>
    {
        public static FocalLengthMetadataProperty Instance { get; } = new FocalLengthMetadataProperty();

        public override string Identifier { get; } = nameof(FocalLengthMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private FocalLengthMetadataProperty() { }

        public override double? Read(IMetadataReader metadataReader)
        {
            return (double?)metadataReader.GetMetadata("System.Photo.FocalLength");
        }

        public override void Write(IMetadataWriter metadataWriter, double? value)
        {
            metadataWriter.SetMetadata("System.Photo.FocalLength", value);
        }
    }
}
