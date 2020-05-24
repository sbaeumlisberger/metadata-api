using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class CopyrightMetadataProperty : IMetadataProperty<string>
    {
        public static CopyrightMetadataProperty Instance { get; } = new CopyrightMetadataProperty();

        public string Identifier { get; } = nameof(CameraManufacturerMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private CopyrightMetadataProperty() { }

        public string Read(IReadMetadata metadataReader)
        {
            return (string)metadataReader.GetMetadata("System.Copyright") ?? string.Empty;
        }

        public void Write(IWriteMetadata metadataWriter, string value)
        {
            metadataWriter.SetMetadata("System.Copyright", value);
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (string)value);
        }
    }
}
