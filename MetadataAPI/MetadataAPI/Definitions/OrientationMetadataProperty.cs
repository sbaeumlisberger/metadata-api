using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;

namespace MetadataAPI.Definitions
{
    public class OrientationMetadataProperty : IMetadataProperty<PhotoOrientation>
    {
        public static OrientationMetadataProperty Instance { get; } = new OrientationMetadataProperty();

        public string Identifier { get; } = nameof(FocalLengthMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private OrientationMetadataProperty() { }

        public PhotoOrientation Read(IReadMetadata metadataReader)
        {
            UInt16? orientation = (UInt16?)metadataReader.GetMetadata("System.Photo.Orientation");
            return orientation != null ? (PhotoOrientation)orientation : PhotoOrientation.Unspecified;
        }

        public void Write(IWriteMetadata metadataWriter, PhotoOrientation value)
        {
            metadataWriter.SetMetadata("System.Photo.Orientation", (UInt16)value);
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (PhotoOrientation)value);
        }
    }
}
