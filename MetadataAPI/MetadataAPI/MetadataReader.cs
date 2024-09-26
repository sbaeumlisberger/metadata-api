using System.Collections.Generic;
using WIC;

namespace MetadataAPI;

public class MetadataReader : IMetadataReader
{
    public IWICBitmapCodecInfo CodecInfo { get; }

    private readonly IWICMetadataQueryReader wicMetadataQueryReader;

    public MetadataReader(IWICMetadataQueryReader wicMetadataQueryReader, IWICBitmapCodecInfo codecInfo)
    {
        CodecInfo = codecInfo;
        this.wicMetadataQueryReader = wicMetadataQueryReader;
    }

    public object? GetMetadata(string key)
    {
        return wicMetadataQueryReader.TryGetMetadataByName(key, out object? value) ? value : null;
    }

    public IMetadataReader? GetMetadataBlock(string key)
    {
        if (GetMetadata(key) is IWICMetadataQueryReader metadataQueryReader)
        {
            return new MetadataReader(metadataQueryReader, CodecInfo);
        }
        return null;
    }

    public IEnumerable<string> GetKeys()
    {
        return wicMetadataQueryReader.GetNames();
    }
}
