using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WIC;

namespace MetadataAPI.WIC
{
    public class WICReadMetadata : IReadMetadata
    {
        public string FileType { get; }

        private readonly IWICMetadataQueryReader wicMetadataQueryReader;

        public WICReadMetadata(string fileType, IWICMetadataQueryReader wicMetadataQueryReader)
        {
            this.FileType = fileType.ToLower();
            this.wicMetadataQueryReader = wicMetadataQueryReader;
        }

        public object GetMetadata(string key)
        {
            return wicMetadataQueryReader.TryGetMetadataByName(key, out object value) ? value : null;
        }

        public IReadMetadata GetMetadataBlock(string key)
        {
            var metadataQueryReader = (IWICMetadataQueryReader)GetMetadata(key);
            if (metadataQueryReader != null)
            {
                return new WICReadMetadata(FileType, metadataQueryReader);
            }
            return null;
        }

        public IEnumerable<string> GetKeys()
        {
            return wicMetadataQueryReader.GetNames();
        }
    }
}
