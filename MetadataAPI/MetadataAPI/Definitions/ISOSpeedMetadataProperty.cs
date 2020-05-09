using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class ISOSpeedMetadataProperty : IMetadataProperty<double?>
    {
        public string Identifier { get; } = nameof(ISOSpeedMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileTypes.JpegExtensions.Concat(FileTypes.TiffExtensions));

        public double? Read(IMetadataReader metadataReader)
        {
            return (double?)metadataReader.GetMetadata("System.Photo.ISOSpeed");
        }

        public void Write(IMetadataWriter metadataWriter, double? value)
        {
            metadataWriter.SetMetadata("System.Photo.ISOSpeed", value);
        }
    }
}
