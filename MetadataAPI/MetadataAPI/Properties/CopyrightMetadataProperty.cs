using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Properties
{
    public class CopyrightMetadataProperty : IMetadataProperty<string>
    {
        public static CopyrightMetadataProperty Instance { get; } = new CopyrightMetadataProperty();

        public string Identifier { get; } = nameof(CopyrightMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private CopyrightMetadataProperty() { }

        public string Read(IMetadataReader metadataReader)
        {
            return (string)metadataReader.GetMetadata("System.Copyright") ?? string.Empty;
        }

        public void Write(IMetadataWriter metadataWriter, string value)
        {
            metadataWriter.SetMetadata("System.Copyright", value);
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object value)
        {
            Write(metadataWriter, (string)value);
        }
    }
}
