using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Properties
{
    public class FocalLengthInFilmMetadataProperty : IMetadataProperty<ushort?>
    {
        public static FocalLengthInFilmMetadataProperty Instance { get; } = new FocalLengthInFilmMetadataProperty();

        public string Identifier { get; } = nameof(FocalLengthInFilmMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private FocalLengthInFilmMetadataProperty() { }

        public ushort? Read(IMetadataReader metadataReader)
        {
            return (ushort?)metadataReader.GetMetadata("System.Photo.FocalLengthInFilm");
        }

        public void Write(IMetadataWriter metadataWriter, ushort? value)
        {
            metadataWriter.SetMetadata("System.Photo.FocalLengthInFilm", value);
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object value)
        {
            Write(metadataWriter, (ushort?)value);
        }
    }
}
