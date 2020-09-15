using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Properties
{
    public class KeywordsMetadataProperty : IMetadataProperty<string[]>
    {
        public static KeywordsMetadataProperty Instance { get; } = new KeywordsMetadataProperty();

        public string Identifier { get; } = nameof(KeywordsMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private KeywordsMetadataProperty() { }

        public string[] Read(IMetadataReader metadataReader)
        {
            return (string[])metadataReader.GetMetadata("System.Keywords") ?? new string[0];
        }

        public void Write(IMetadataWriter metadataWriter, string[] value)
        {
            metadataWriter.SetMetadata("System.Keywords", value?.ToArray());
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object value)
        {
            Write(metadataWriter, (string[])value);
        }
    }
}
