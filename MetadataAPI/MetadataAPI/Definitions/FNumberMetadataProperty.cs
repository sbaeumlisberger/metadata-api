using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetToolkit.Foundation;

namespace MetadataAPI.Definitions
{
    public class FNumberMetadataProperty : IMetadataProperty<Fraction?>
    {
        public static FNumberMetadataProperty Instance { get; } = new FNumberMetadataProperty();

        public string Identifier { get; } = nameof(FNumberMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private FNumberMetadataProperty() { }

        public Fraction? Read(IReadMetadata metadataReader)
        {
            if (metadataReader.GetMetadata("System.Photo.FNumberNumerator") is UInt32 numerator
                && metadataReader.GetMetadata("System.Photo.FNumberDenominator") is UInt32 denominator)
            {
                return new Fraction(numerator, denominator);
            }
            return null;
        }

        public void Write(IWriteMetadata metadataWriter, Fraction? value)
        {
            metadataWriter.SetMetadata("System.Photo.FNumberNumerator", (UInt32?)value?.Numerator);
            metadataWriter.SetMetadata("System.Photo.FNumberDenominator", (UInt32?)value?.Denominator);
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
