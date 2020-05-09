using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public class WICMetadataReader : IMetadataReader
    {
        public string FileType { get; set; }

        private IWICMetadataQueryReader wicMetadataQueryReader;

        public void SetWICMetadataQueryReader(IWICMetadataQueryReader wicMetadataQueryReader)
        {
            this.wicMetadataQueryReader = wicMetadataQueryReader;
        }

        public object GetMetadata(string key)
        {
            if (wicMetadataQueryReader.TryGetMetadataByName(key, out var value))
            {
                return value;
            }
            return null;
        }

        public IMetadataReader GetMetadataBlock(string key)
        {
            var metadataReader = new WICMetadataReader();
            var metadataQueryReader = (IWICMetadataQueryReader)GetMetadata(key);
            metadataReader.SetWICMetadataQueryReader(metadataQueryReader);
            return metadataReader;
        }

        public IEnumerable<string> GetKeys()
        {
            return wicMetadataQueryReader.GetNames();
        }
    }
}
