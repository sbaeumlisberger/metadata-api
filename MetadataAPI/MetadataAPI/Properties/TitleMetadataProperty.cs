using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WIC;

namespace MetadataAPI.Properties
{
    public class TitleMetadataProperty : MetadataPropertyBase<string>
    {
        public static TitleMetadataProperty Instance { get; } = new TitleMetadataProperty();

        public override string Identifier { get; } = nameof(TitleMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private TitleMetadataProperty() { }

        public override string Read(IMetadataReader metadataReader)
        {
            return (string?)metadataReader.GetMetadata("System.Title") ?? string.Empty;
        }

        public override void Write(IMetadataWriter metadataWriter, string value)
        {
            if (value != string.Empty)
            {
                metadataWriter.SetMetadata("System.Title", value);
            }
            else
            {
                metadataWriter.SetMetadata("System.Title", null);
            }
        }
    }
}
