using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI
{
    public static class MetadataWriterExtension
    {
        public static void SetProperty<T>(this IMetadataWriter metadataWriter, IMetadataProperty<T> property, T value) 
        {
            property.Write(metadataWriter, value);
        }

        public static void SetProperties(this IMetadataWriter metadataWriter, MetadataPropertySet propertySet)
        {
            foreach ((var property, var value) in propertySet) 
            {
                property.Write(metadataWriter, value);
            }
        }
    }
}
