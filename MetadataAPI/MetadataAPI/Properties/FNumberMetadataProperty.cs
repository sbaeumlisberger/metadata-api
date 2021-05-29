using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;
using WIC;

namespace MetadataAPI.Properties
{
    public class FNumberMetadataProperty : MetadataPropertyBase<Fraction?>
    {
        public static FNumberMetadataProperty Instance { get; } = new FNumberMetadataProperty();

        public override string Identifier { get; } = nameof(FNumberMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private FNumberMetadataProperty() { }

        public override Fraction? Read(IMetadataReader metadataReader)
        {
            if (metadataReader.GetMetadata("System.Photo.FNumberNumerator") is UInt32 numerator
                && metadataReader.GetMetadata("System.Photo.FNumberDenominator") is UInt32 denominator)
            {
                return new Fraction(numerator, denominator);
            }
            return null;
        }

        public override void Write(IMetadataWriter metadataWriter, Fraction? value)
        {
            metadataWriter.SetMetadata("System.Photo.FNumberNumerator", (UInt32?)value?.Numerator);
            metadataWriter.SetMetadata("System.Photo.FNumberDenominator", (UInt32?)value?.Denominator);
        }

    }
}
