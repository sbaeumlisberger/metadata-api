using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI
{
    public interface IMetadataWriter : IMetadataReader
    {
        void SetMetadata(string name, object value);
    }
}
