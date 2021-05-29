using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public interface IMetadataReader
    {
        IWICBitmapCodecInfo CodecInfo { get; }

        object? GetMetadata(string key);

        IEnumerable<string> GetKeys();

        IMetadataReader? GetMetadataBlock(string key);
    }

}
