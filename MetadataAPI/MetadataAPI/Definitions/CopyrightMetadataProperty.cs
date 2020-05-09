using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class CopyrightMetadataProperty : IMetadataProperty<string>
    {
        public string Identifier { get; } = nameof(CameraManufacturerMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileTypes.JpegExtensions.Concat(FileTypes.TiffExtensions));

        public string Read(IMetadataReader metadataReader)
        {
            return (string)metadataReader.GetMetadata("System.Copyright") ?? string.Empty;
        }

        public void Write(IMetadataWriter metadataWriter, string value)
        {
            metadataWriter.SetMetadata("System.Copyright", value);
        }
    }
}
