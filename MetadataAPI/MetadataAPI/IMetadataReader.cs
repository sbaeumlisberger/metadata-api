using System.Collections.Generic;
using WIC;

namespace MetadataAPI;

public interface IMetadataReader
{
    IWICBitmapCodecInfo CodecInfo { get; }

    object? GetMetadata(string key);

    IEnumerable<string> GetKeys();

    IMetadataReader? GetMetadataBlock(string key);
}
