using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MetadataAPI.Provider
{
    public interface IMetadataReader
    {
        IReadMetadata SetStream(Stream stream, string fileType);
    }
}
