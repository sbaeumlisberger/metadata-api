using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MetadataAPI
{
    public interface IReadMetadata
    {
        string FileType { get; }

        object GetMetadata(string key);

        IEnumerable<string> GetKeys();

        IReadMetadata GetMetadataBlock(string key);
    }

}
