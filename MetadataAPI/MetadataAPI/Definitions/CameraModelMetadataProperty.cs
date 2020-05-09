using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class CameraModelMetadataProperty : IMetadataProperty<string>
    {
        public string Identifier { get; } = nameof(CameraModelMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileTypes.JpegExtensions.Concat(FileTypes.TiffExtensions));

        public string Read(IMetadataReader metadataReader)
        {
            return (string)metadataReader.GetMetadata("System.Photo.CameraModel") ?? string.Empty;
        }

        public void Write(IMetadataWriter metadataWriter, string value)
        {
            metadataWriter.SetMetadata("System.Photo.CameraModel", value);
        }
    }
}
