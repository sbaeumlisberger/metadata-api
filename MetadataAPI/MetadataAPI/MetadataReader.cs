using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public class MetadataReader : IMetadataReader
    {
        public string FileType { get; }

        private readonly IWICMetadataQueryReader wicMetadataQueryReader;

        public MetadataReader(IWICMetadataQueryReader wicMetadataQueryReader, string fileType)
        {
            this.wicMetadataQueryReader = wicMetadataQueryReader;
            FileType = fileType.ToLower();
        }

        public object GetMetadata(string key)
        {
            return wicMetadataQueryReader.TryGetMetadataByName(key, out object value) ? value : null;
        }

        public IMetadataReader GetMetadataBlock(string key)
        {
            if (GetMetadata(key) is IWICMetadataQueryReader metadataQueryReader)
            {
                return new MetadataReader(metadataQueryReader, FileType);
            }
            return null;
        }

        public IEnumerable<string> GetKeys()
        {
            return wicMetadataQueryReader.GetNames();
        }
    }
}
