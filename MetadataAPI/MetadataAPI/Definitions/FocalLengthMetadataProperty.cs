using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class FocalLengthMetadataProperty : IMetadataProperty<double?>
    {
        public static FocalLengthMetadataProperty Instance { get; } = new FocalLengthMetadataProperty();

        public string Identifier { get; } = nameof(FocalLengthMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private FocalLengthMetadataProperty() { }

        public double? Read(IReadMetadata metadataReader)
        {
            return (double?)metadataReader.GetMetadata("System.Photo.FocalLength");
        }

        public void Write(IWriteMetadata metadataWriter, double? value)
        {
            metadataWriter.SetMetadata("System.Photo.FocalLength", value);
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (double?)value);
        }
    }
}
