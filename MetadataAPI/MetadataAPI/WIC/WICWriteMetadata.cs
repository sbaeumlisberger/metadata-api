using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WIC;

namespace MetadataAPI.WIC
{
    public class WICWriteMetadata : IWriteMetadata
    {
        public string FileType { get; }

        public IReadOnlyList<(string, object)> Requests => requests;

        private readonly List<(string, object)> requests = new List<(string, object)>();

        private readonly IWICMetadataQueryWriter wicMetadataQueryWriter;

        public WICWriteMetadata(string fileType, IWICMetadataQueryWriter wicMetadataQueryWriter)
        {
            this.FileType = fileType.ToLower();
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

        public IReadMetadata GetMetadataBlock(string key)
        {
            var metadataQueryReader = (IWICMetadataQueryReader)GetMetadata(key);
            return new WICReadMetadata(FileType, metadataQueryReader);
        }

        public IEnumerable<string> GetKeys()
        {
            return wicMetadataQueryWriter.GetNames();
        }

        public void SetMetadata(string name, object value)
        {
            requests.Add((name, value));

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
