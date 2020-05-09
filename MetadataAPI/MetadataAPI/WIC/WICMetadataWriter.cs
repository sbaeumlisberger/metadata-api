using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public class WICMetadataWriter : IMetadataWriter
    {
        public string FileType { get; set; }

        private IWICMetadataQueryWriter wicMetadataQueryWriter;

        public void SetWICMetadataQueryWriter(IWICMetadataQueryWriter wicMetadataQueryWriter)
        {
            this.wicMetadataQueryWriter = wicMetadataQueryWriter;
        }

        public object GetMetadata(string key)
        {
            if (wicMetadataQueryWriter.TryGetMetadataByName(key, out var value))
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
            return wicMetadataQueryWriter.GetNames();
        }

        public void SetMetadata(string name, object value)
        {
            if (value is null)
            {
                wicMetadataQueryWriter.RemoveMetadataByName(name);
            }
            else
            {
               wicMetadataQueryWriter.SetMetadataByName(name, value);                
            }
        }
    }
}
