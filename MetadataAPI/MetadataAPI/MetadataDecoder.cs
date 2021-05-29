using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public class MetadataDecoder : IMetadataReader
    {
        public IWICBitmapCodecInfo CodecInfo { get; }

        private readonly WICImagingFactory wic = new WICImagingFactory();

        private readonly MetadataReader metadataReader;

        [Obsolete]
        public MetadataDecoder(Stream stream, string fileType, WICDecodeOptions decodeOptions = WICDecodeOptions.WICDecodeMetadataCacheOnDemand)
            : this(stream, decodeOptions)
        { }

        public MetadataDecoder(Stream stream, WICDecodeOptions decodeOptions = WICDecodeOptions.WICDecodeMetadataCacheOnDemand)
        {
            var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), decodeOptions);

            CodecInfo = decoder.GetDecoderInfo();

            var frame = decoder.GetFrame(0);

            var metadataQueryReader = frame.GetMetadataQueryReader();

            metadataReader = new MetadataReader(metadataQueryReader, CodecInfo);
        }

        public object? GetMetadata(string key)
        {
            return metadataReader.GetMetadata(key);
        }

        public IEnumerable<string> GetKeys()
        {
            return metadataReader.GetKeys();
        }

        public IMetadataReader? GetMetadataBlock(string key)
        {
            return metadataReader.GetMetadataBlock(key);
        }
    }
}
