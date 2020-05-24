using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetToolkit.Foundation;

namespace MetadataAPI.Definitions
{
    public class ExposureTimeMetadataProperty : IMetadataProperty<Fraction?>
    {
        public static ExposureTimeMetadataProperty Instance { get; } = new ExposureTimeMetadataProperty();

        public string Identifier { get; } = nameof(ExposureTimeMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private ExposureTimeMetadataProperty() { }

        public Fraction? Read(IReadMetadata metadataReader)
        {
            if (metadataReader.GetMetadata("System.Photo.ExposureTimeNumerator") is UInt32 numerator
                && metadataReader.GetMetadata("System.Photo.ExposureTimeDenominator") is UInt32 denominator)
            {
                return new Fraction(numerator, denominator);
            }
            return null;
        }

        public void Write(IWriteMetadata metadataWriter, Fraction? value)
        {
            metadataWriter.SetMetadata("System.Photo.ExposureTimeNumerator", (UInt32?)value?.Numerator);
            metadataWriter.SetMetadata("System.Photo.ExposureTimeDenominator", (UInt32?)value?.Denominator);
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (Fraction?)value);
        }
    }
}
