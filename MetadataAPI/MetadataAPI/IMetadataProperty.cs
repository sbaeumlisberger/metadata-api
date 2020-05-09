using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI
{
    public interface IMetadataProperty<T> : IReadonlyMetadataProperty<T>
    {
        void Write(IMetadataWriter metadataWriter, T value);
    }
}
