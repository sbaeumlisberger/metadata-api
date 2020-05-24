using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI.WIC;
using Windows.Storage;

namespace MetadataAPI.UWP
{
    public static class StorageFileExtension
    {
        //public static async Task<MetadataView> ReadMetadataAsync(this IStorageFile file, IEnumerable<IReadonlyMetadataProperty> properties)
        //{
        //    using (var stream = await file.OpenAsync(FileAccessMode.Read).AsTask().ConfigureAwait(false))
        //    {
        //        var metadataReader = new WICMetadataReader();
        //        metadataReader.SetStream(stream.AsStream(), file.FileType);

        //        return new MetadataView(properties.ToDictionary(
        //            property => property.Identifier,
        //            property => metadataReader.GetMetadata(property)));               
        //    }
        //}

        public static async Task<T> ReadMetadataAsync<T>(this StorageFile file, IReadonlyMetadataProperty<T> property)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read).AsTask().ConfigureAwait(false))
            {
                var metadataReader = new MetadataReader(stream.AsStream(), file.FileType);
                return metadataReader.GetMetadata(property);
            }
        }

        //public static async Task WriteMetadataAsync(this StorageFile file, IEnumerable<(IMetadataProperty Property, object Value)> properties)
        //{
        //    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
        //    {
        //        var metadataWriter = new WICMetadataWriter();
        //        metadataWriter.SetStream(stream.AsStream(), file.FileType);
        //        foreach (var (property, value) in properties)
        //        {
        //            metadataWriter.SetMetadata(property, value);
        //        }
        //    }
        //}

        public static async Task WriteMetadataAsync<T>(this StorageFile file, IMetadataProperty<T> property, T value)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
            {
                var metadataWriter = new MetadataWriter(stream.AsStream(), file.FileType);
                metadataWriter.SetMetadata(property, value);
            }
        }

    }

}
