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

        IReadOnlyCollection<Guid> SupportedFormats { get; }

        object? Read(IMetadataReader metadataReader);
    }

    public interface IReadonlyMetadataProperty<T> : IReadonlyMetadataProperty
    {
        new T Read(IMetadataReader metadataReader);
    }

}
