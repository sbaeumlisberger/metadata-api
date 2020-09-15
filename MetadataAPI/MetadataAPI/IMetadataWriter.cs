using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public interface IMetadataWriter : IMetadataReader
    {
        void SetMetadata(string name, object value);
    }
}
