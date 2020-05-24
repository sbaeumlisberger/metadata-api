using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI
{
    public interface IReadonlyMetadataProperty
    {
        string Identifier { get; }

        IReadOnlyCollection<string> SupportedFileTypes { get; }

        object Read(IReadMetadata metadataReader);
    }

    public interface IReadonlyMetadataProperty<T> : IReadonlyMetadataProperty
    {
        new T Read(IReadMetadata metadataReader);
    }

}
