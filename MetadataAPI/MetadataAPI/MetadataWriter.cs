using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public class MetadataWriter : IMetadataWriter
    {
        public IWICBitmapCodecInfo CodecInfo { get; }

        private readonly IWICMetadataQueryWriter wicMetadataQueryWriter;

        public MetadataWriter(IWICMetadataQueryWriter wicMetadataQueryWriter, IWICBitmapCodecInfo codecInfo)
        {
            CodecInfo = codecInfo;
            this.wicMetadataQueryWriter = wicMetadataQueryWriter;
        }

        public object? GetMetadata(string key)
        {
            return wicMetadataQueryWriter.TryGetMetadataByName(key, out object? value) ? value : null;
        }

        public IMetadataReader? GetMetadataBlock(string key)
        {
            var metadataQueryReader = (IWICMetadataQueryReader?)GetMetadata(key);
            if (metadataQueryReader != null)
            {
                return new MetadataReader(metadataQueryReader, CodecInfo);
            }
            return null;
        }

        public IEnumerable<string> GetKeys()
        {
            return wicMetadataQueryWriter.GetNames();
        }

        public void SetMetadata(string name, object? value)
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
