using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class KeywordsMetadataProperty : IMetadataProperty<string[]>
    {
        public string Identifier { get; } = nameof(KeywordsMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileTypes.JpegExtensions.Concat(FileTypes.TiffExtensions));

        public string[] Read(IMetadataReader metadataReader)
        {
            return (string[])metadataReader.GetMetadata("System.Keywords") ?? new string[0];
        }

        public void Write(IMetadataWriter metadataWriter, string[] value)
        {
            metadataWriter.SetMetadata("System.Keywords", value.ToArray());
        }
    }
}
