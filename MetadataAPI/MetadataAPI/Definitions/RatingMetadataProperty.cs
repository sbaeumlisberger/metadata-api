using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Definitions
{
    public class RatingMetadataProperty : IMetadataProperty<int>
    {
        public static RatingMetadataProperty Instance { get; } = new RatingMetadataProperty();

        public string Identifier { get; } = nameof(RatingMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private RatingMetadataProperty() { }

        public int Read(IReadMetadata metadataReader)
        {
            if (metadataReader.GetMetadata("System.SimpleRating") is UInt32 simpleRating)
            {
                return (int)simpleRating;
            }
            else if (metadataReader.GetMetadata("System.Rating") is UInt32 rating)
            {
                if (rating >= 99) return 5;
                if (rating >= 75) return 4;
                if (rating >= 50) return 3;
                if (rating >= 25) return 2;
                if (rating >= 1) return 1;
            }
            return 0;
        }

        public void Write(IWriteMetadata metadataWriter, int value)
        {
            if (value < 0 || value > 5) throw new ArgumentOutOfRangeException(nameof(value), "Must be in the range 0 to 5.");
            // https://msdn.microsoft.com/en-us/library/windows/desktop/bb787554(v=vs.85).aspx
            UInt32[] simpleRatingToRatingTable = new UInt32[] { 0, 1, 25, 50, 75, 99 };
            metadataWriter.SetMetadata("System.SimpleRating", (UInt32)value);
            metadataWriter.SetMetadata("System.Rating", simpleRatingToRatingTable[value]);
        }

        object IReadonlyMetadataProperty.Read(IReadMetadata metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IWriteMetadata metadataWriter, object value)
        {
            Write(metadataWriter, (int)value);
        }

    }
}
