using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI
{   
    public interface IReadonlyMetadataProperty<T>
    {
        string Identifier { get; }

        IReadOnlyCollection<string> SupportedFileTypes { get; }

        T Read(IMetadataReader metadataReader);
    }
}
