using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class ISOSpeedMetadataProperty : IMetadataProperty<double?>
    {
        public static ISOSpeedMetadataProperty Instance { get; } = new ISOSpeedMetadataProperty();

        public string Identifier { get; } = nameof(ISOSpeedMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private ISOSpeedMetadataProperty() { }

        public double? Read(IReadMetadata metadataReader)
        {
            return (double?)metadataReader.GetMetadata("System.Photo.ISOSpeed");
        }

        public void Write(IWriteMetadata metadataWriter, double? value)
        {
            metadataWriter.SetMetadata("System.Photo.ISOSpeed", value);
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
