using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI
{
    public interface IMetadataProperty : IReadonlyMetadataProperty
    {
        void Write(IMetadataWriter metadataWriter, object? value);
    }

    public interface IMetadataProperty<T> : IMetadataProperty, IReadonlyMetadataProperty<T>
    {
        void Write(IMetadataWriter metadataWriter, T value);
    }
}
