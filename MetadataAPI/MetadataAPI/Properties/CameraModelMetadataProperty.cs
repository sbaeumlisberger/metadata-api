using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Properties
{
    public class CameraModelMetadataProperty : IMetadataProperty<string?>
    {
        public static CameraModelMetadataProperty Instance { get; } = new CameraModelMetadataProperty();

        public string Identifier { get; } = nameof(CameraModelMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private CameraModelMetadataProperty() { }

        public string? Read(IMetadataReader metadataReader)
        {
            return (string?)metadataReader.GetMetadata("System.Photo.CameraModel") ?? string.Empty;
        }

        public void Write(IMetadataWriter metadataWriter, string? value)
        {
            metadataWriter.SetMetadata("System.Photo.CameraModel", value);
        }

        object? IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object? value)
        {
            Write(metadataWriter, (string?)value);
        }
    }
}
