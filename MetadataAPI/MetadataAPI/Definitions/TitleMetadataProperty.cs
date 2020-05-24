using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class TitleMetadataProperty : IMetadataProperty<string>
    {
        public static TitleMetadataProperty Instance { get; } = new TitleMetadataProperty();

        public string Identifier { get; } = nameof(TitleMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private TitleMetadataProperty() { }

        public string Read(IReadMetadata metadataReader)
        {
            return (string)metadataReader.GetMetadata("System.Title") ?? string.Empty;
        }

        public void Write(IWriteMetadata metadataWriter, string value)
        {
            metadataWriter.SetMetadata("System.Title", value);
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
