using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Properties
{
    public class ISOSpeedMetadataProperty : IMetadataProperty<ushort?>
    {
        public static ISOSpeedMetadataProperty Instance { get; } = new ISOSpeedMetadataProperty();

        public string Identifier { get; } = nameof(ISOSpeedMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private ISOSpeedMetadataProperty() { }

        public ushort? Read(IMetadataReader metadataReader)
        {
            return (ushort?)metadataReader.GetMetadata("System.Photo.ISOSpeed");
        }

        public void Write(IMetadataWriter metadataWriter, ushort? value)
        {
            metadataWriter.SetMetadata("System.Photo.ISOSpeed", value);
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object value)
        {
            Write(metadataWriter, (ushort?)value);
        }
    }
}
