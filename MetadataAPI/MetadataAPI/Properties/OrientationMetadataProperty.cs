using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;

namespace MetadataAPI.Properties
{
    public class OrientationMetadataProperty : IMetadataProperty<PhotoOrientation>
    {
        public static OrientationMetadataProperty Instance { get; } = new OrientationMetadataProperty();

        public string Identifier { get; } = nameof(OrientationMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private OrientationMetadataProperty() { }

        public PhotoOrientation Read(IMetadataReader metadataReader)
        {
            UInt16? orientation = (UInt16?)metadataReader.GetMetadata("System.Photo.Orientation");
            return orientation != null ? (PhotoOrientation)orientation : PhotoOrientation.Unspecified;
        }

        public void Write(IMetadataWriter metadataWriter, PhotoOrientation value)
        {
            if (value == PhotoOrientation.Unspecified)
            {
                metadataWriter.SetMetadata("System.Photo.Orientation", null);
            }
            else
            {
                metadataWriter.SetMetadata("System.Photo.Orientation", (UInt16)value);
            }
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object? value)
        {
            Write(metadataWriter, (PhotoOrientation)(value ?? PhotoOrientation.Unspecified));
        }
    }
}
