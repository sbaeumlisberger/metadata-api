using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI.Provider
{
    public interface IMetadataProvider
    {

        IMetadataReader CreateMetadataReader();

        IMetadataWriter CreateMetadataWriter(); 
        
    }

}
