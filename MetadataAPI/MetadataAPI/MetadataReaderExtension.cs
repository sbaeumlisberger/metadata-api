using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI
{
    public static class MetadataReaderExtension
    {
        public static object? GetProperty(this IMetadataReader metadataReader, IReadonlyMetadataProperty property)
        {
            return property.Read(metadataReader);
        }

        public static T GetProperty<T>(this IMetadataReader metadataReader, IReadonlyMetadataProperty<T> property) 
        {
            return property.Read(metadataReader);
        }
    }
}
