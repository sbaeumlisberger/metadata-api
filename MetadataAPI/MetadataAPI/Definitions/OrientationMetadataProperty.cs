using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;

namespace MetadataAPI.Definitions
{
    public class OrientationMetadataProperty : IMetadataProperty<PhotoOrientation>
    {
        public string Identifier { get; } = nameof(FocalLengthMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileTypes.JpegExtensions.Concat(FileTypes.TiffExtensions));

        public PhotoOrientation Read(IMetadataReader metadataReader)
        {
            UInt16? orientation = (UInt16?)metadataReader.GetMetadata("System.Photo.Orientation");
            return orientation != null ? (PhotoOrientation)orientation : PhotoOrientation.Unspecified;
        }

        public void Write(IMetadataWriter metadataWriter, PhotoOrientation value)
        {
            metadataWriter.SetMetadata("System.Photo.Orientation", (UInt16)value);
        }
    }
}
