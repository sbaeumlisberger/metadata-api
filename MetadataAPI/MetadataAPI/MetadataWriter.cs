using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI.Provider;
using MetadataAPI.WIC;

namespace MetadataAPI
{
    public class MetadataWriter
    {

        private readonly IMetadataWriter metadataWriter;

        private IWriteMetadata writeMetadata;

        public MetadataWriter()
        {
            metadataWriter = new WICMetadataProvider().CreateMetadataWriter();
        }

        public MetadataWriter(Stream stream, string fileType)
        {
            metadataWriter = new WICMetadataProvider().CreateMetadataWriter();
            SetStream(stream, fileType);
        }

        public MetadataWriter(IMetadataProvider provider)        
        {
            metadataWriter = provider.CreateMetadataWriter();
        }

        public MetadataWriter(IMetadataProvider provider, Stream stream, string fileType)
        {
            metadataWriter = provider.CreateMetadataWriter();
            SetStream(stream, fileType);
        }

        public void SetStream(Stream stream, string fileType)
        {
            writeMetadata = metadataWriter.SetStream(stream, fileType);
        }

        public void SetMetadata<T>(IMetadataProperty<T> property, T value)
        {
            if (writeMetadata is null)
            {
                throw new InvalidOperationException("No stream has been set.");
            }

            property.Write(writeMetadata, value);
        }

        public void SetMetadata(IMetadataProperty property, object value)
        {
            if (writeMetadata is null)
            {
                throw new InvalidOperationException("No stream has been set.");
            }

            property.Write(writeMetadata, value);
        }

        public Task CommitAsync() 
        {
            return metadataWriter.CommitAsync();
        }

    }
}
