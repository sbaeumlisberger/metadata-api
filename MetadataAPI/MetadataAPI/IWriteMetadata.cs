using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MetadataAPI
{
    public interface IWriteMetadata : IReadMetadata
    {
        void SetMetadata(string name, object value);
    }
}
