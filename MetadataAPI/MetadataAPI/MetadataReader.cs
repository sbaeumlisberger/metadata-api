using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MetadataAPI.Provider;
using MetadataAPI.WIC;

namespace MetadataAPI
{
    public class MetadataReader
    {

        private readonly IMetadataReader metadataReader;

        private IReadMetadata readMetadata;

        public MetadataReader()
        {
            metadataReader = new WICMetadataProvider().CreateMetadataReader();
        }

        public MetadataReader(Stream stream, string fileType)
        {
            metadataReader = new WICMetadataProvider().CreateMetadataReader();
            SetStream(stream, fileType);
        }

        public MetadataReader(IMetadataProvider provider)
        {
            metadataReader = provider.CreateMetadataReader();
        }

        public MetadataReader(IMetadataProvider provider, Stream stream, string fileType)
        {
            metadataReader = provider.CreateMetadataReader();
            SetStream(stream, fileType);
        }

        public void SetStream(Stream stream, string fileType)
        {
            readMetadata = metadataReader.SetStream(stream, fileType);
        }

        public T GetMetadata<T>(IReadonlyMetadataProperty<T> property)
        {
            if (readMetadata is null)
            {
                throw new InvalidOperationException("No stream has been set.");
            }

            return property.Read(readMetadata);
        }

    }
}
