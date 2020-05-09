using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetToolkit.Foundation;

namespace MetadataAPI.Definitions
{
    public class ExposureTimeMetadataProperty : IMetadataProperty<Fraction?>
    {
        public string Identifier { get; } = nameof(ExposureTimeMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileTypes.JpegExtensions.Concat(FileTypes.TiffExtensions));

        public Fraction? Read(IMetadataReader metadataReader)
        {
            if (metadataReader.GetMetadata("System.Photo.ExposureTimeNumerator") is UInt32 numerator
                && metadataReader.GetMetadata("System.Photo.ExposureTimeDenominator") is UInt32 denominator)
            {
                return new Fraction(numerator, denominator);
            }
            return null;
        }

        public void Write(IMetadataWriter metadataWriter, Fraction? value)
        {
            metadataWriter.SetMetadata("System.Photo.ExposureTimeNumerator", (UInt32?)value?.Numerator);
            metadataWriter.SetMetadata("System.Photo.ExposureTimeDenominator", (UInt32?)value?.Denominator);
        }
    }
}
