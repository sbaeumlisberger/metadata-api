using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI;

namespace MetadataAPITest
{
    public static class TestUtil
    {

        public static async Task WriteMetadataPropertiesAync(string filePath, MetadataPropertySet propertySet)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataEncoder = new MetadataEncoder(stream);
                metadataEncoder.SetProperties(propertySet);
                await metadataEncoder.EncodeAsync().ConfigureAwait(false);
            }
        }

        public static async Task WriteMetadataPropertyAync<T>(string filePath, IMetadataProperty<T> property, T value)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataEncoder = new MetadataEncoder(stream);
                metadataEncoder.SetProperty(property, value);
                await metadataEncoder.EncodeAsync().ConfigureAwait(false);
            }
        }

        public static async Task<T> ReadMetadataPropertyAync<T>(string filePath, IReadonlyMetadataProperty<T> property)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataEncoder = new MetadataEncoder(stream);
                return metadataEncoder.GetProperty(property);
            }
        }
    }
}
