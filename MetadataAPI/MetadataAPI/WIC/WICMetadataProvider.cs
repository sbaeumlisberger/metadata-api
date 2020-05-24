using System;
using System.Collections.Generic;
using System.Text;
using MetadataAPI.Provider;

namespace MetadataAPI.WIC
{
    public class WICMetadataProvider : IMetadataProvider
    {
        public IMetadataReader CreateMetadataReader()
        {
            return new WICMetadataReader();
        }

        public IMetadataWriter CreateMetadataWriter()
        {
            return new WICMetadataWriter();
        }
    }
}
