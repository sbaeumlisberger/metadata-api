using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public class MetadataDecoder : IMetadataReader
    {
        public string FileType { get; }

        private readonly WICImagingFactory wic = new WICImagingFactory();

        private readonly MetadataReader metadataReader;              

        public MetadataDecoder(Stream stream, string fileType, WICDecodeOptions decodeOptions = WICDecodeOptions.WICDecodeMetadataCacheOnDemand)
        {
            FileType = fileType;

            var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), decodeOptions);

            var frame = decoder.GetFrame(0);

            var metadataQueryReader = frame.GetMetadataQueryReader();

            metadataReader = new MetadataReader(metadataQueryReader, fileType);
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
