using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MetadataAPI.Provider
{
    public interface IMetadataWriter
    {
        IWriteMetadata SetStream(Stream stream, string fileType);

        Task CommitAsync();       
    }
}
