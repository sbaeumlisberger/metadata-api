﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class FocalLengthInFilmMetadataProperty : IMetadataProperty<double?>
    {
        public static FocalLengthInFilmMetadataProperty Instance { get; } = new FocalLengthInFilmMetadataProperty();

        public string Identifier { get; } = nameof(FocalLengthInFilmMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private FocalLengthInFilmMetadataProperty() { }

        public double? Read(IReadMetadata metadataReader)
        {
            return (double?)metadataReader.GetMetadata("System.Photo.FocalLengthInFilm");
        }

        public void Write(IWriteMetadata metadataWriter, double? value)
        {
            metadataWriter.SetMetadata("System.Photo.FocalLengthInFilm", value);
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (double?)value);
        }
    }
}
