using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class CameraManufacturerMetadataProperty : IMetadataProperty<string>
    {
        public static CameraManufacturerMetadataProperty Instance { get; } = new CameraManufacturerMetadataProperty();

        public string Identifier { get; } = nameof(CameraManufacturerMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private CameraManufacturerMetadataProperty() { }

        public string Read(IReadMetadata metadataReader)
        {
            return (string)metadataReader.GetMetadata("System.Photo.CameraManufacturer") ?? string.Empty;
        }

        public void Write(IWriteMetadata metadataWriter, string value)
        {
            metadataWriter.SetMetadata("System.Photo.CameraManufacturer", value);
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
