using System.IO;
using MetadataAPI.Provider;
using WIC;

namespace MetadataAPI.WIC
{
    public class WICMetadataReader : IMetadataReader
    {

        private readonly WICImagingFactory wic = new WICImagingFactory();

        public IReadMetadata SetStream(Stream stream, string fileType)
        {
            var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);

            var frame = decoder.GetFrame(0);

            var metadataQueryReader = frame.GetMetadataQueryReader();

            return new WICReadMetadata(fileType, metadataQueryReader);
        }

    }
}
