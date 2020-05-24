using System;
using System.Collections.Generic;
using System.Text;
using DotNetToolkit.Collections;

namespace MetadataAPI
{
    public class MetadataView
    {

        public IDictionary<string, object> Source { get; }

        public MetadataView(IDictionary<string, object> source)
        {
            Source = source;
        }

        public T Get<T>(IReadonlyMetadataProperty<T> propertyDefinition)
        {
            return (T)Source.GetOrDefault(propertyDefinition.Identifier);
        }
    }
}
