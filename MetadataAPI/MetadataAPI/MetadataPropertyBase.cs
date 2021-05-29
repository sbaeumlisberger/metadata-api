using System;
using System.Collections.Generic;
using System.Text;
using MetadataAPI.Utils;

namespace MetadataAPI
{
    public abstract class MetadataPropertyBase<T> : IMetadataProperty<T>
    {
        public abstract string Identifier { get; }

        public abstract IReadOnlyCollection<Guid> SupportedFormats { get; }

        public abstract T Read(IMetadataReader metadataReader);

        public abstract void Write(IMetadataWriter metadataWriter, T value);

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object? value)
        {
            if (!TypeUtil.IsAssignableTo<T>(value)) 
            {
                throw new ArgumentException("Invalid type.", nameof(value));
            }
            Write(metadataWriter, (T)value!);
        }

        object? IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }
    }
}
