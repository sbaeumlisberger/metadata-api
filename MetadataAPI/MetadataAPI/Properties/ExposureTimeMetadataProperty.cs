using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;
using WIC;

namespace MetadataAPI.Properties
{
    public class ExposureTimeMetadataProperty : MetadataPropertyBase<Fraction?>
    {
        public static ExposureTimeMetadataProperty Instance { get; } = new ExposureTimeMetadataProperty();

        public override string Identifier { get; } = nameof(ExposureTimeMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private ExposureTimeMetadataProperty() { }

        public override Fraction? Read(IMetadataReader metadataReader)
        {
            if (metadataReader.GetMetadata("System.Photo.ExposureTimeNumerator") is UInt32 numerator
                && metadataReader.GetMetadata("System.Photo.ExposureTimeDenominator") is UInt32 denominator)
            {
                return new Fraction(numerator, denominator);
            }
            return null;
        }

        public override void Write(IMetadataWriter metadataWriter, Fraction? value)
        {
            metadataWriter.SetMetadata("System.Photo.ExposureTimeNumerator", (UInt32?)value?.Numerator);
            metadataWriter.SetMetadata("System.Photo.ExposureTimeDenominator", (UInt32?)value?.Denominator);
        }

    }
}
