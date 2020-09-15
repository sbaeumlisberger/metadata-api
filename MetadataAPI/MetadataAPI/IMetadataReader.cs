using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MetadataAPI
{
    public interface IMetadataReader
    {
        string FileType { get; }

        object GetMetadata(string key);

        IEnumerable<string> GetKeys();

        IMetadataReader GetMetadataBlock(string key);
    }

}
