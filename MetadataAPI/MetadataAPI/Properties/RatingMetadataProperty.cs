using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataAPI.Properties
{
    public class RatingMetadataProperty : IMetadataProperty<int>
    {
        public static RatingMetadataProperty Instance { get; } = new RatingMetadataProperty();

        public string Identifier { get; } = nameof(RatingMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileExtensions.Jpeg.Concat(FileExtensions.Tiff));

        private RatingMetadataProperty() { }

        public int Read(IMetadataReader metadataReader)
        {
            if (metadataReader.GetMetadata("System.SimpleRating") is object simpleRating)
            {
                return Convert.ToInt32(simpleRating);
            }
            else if (metadataReader.GetMetadata("System.Rating") is object rating)
            {
                int ratingValue = Convert.ToInt32(rating);
                if (ratingValue >= 99) return 5;
                if (ratingValue >= 75) return 4;
                if (ratingValue >= 50) return 3;
                if (ratingValue >= 25) return 2;
                if (ratingValue >= 1) return 1;
            }
            return 0;
        }

        public void Write(IMetadataWriter metadataWriter, int value)
        {
            if (value < 0 || value > 5) throw new ArgumentOutOfRangeException(nameof(value), "Must be in the range 0 to 5.");
            // https://msdn.microsoft.com/en-us/library/windows/desktop/bb787554(v=vs.85).aspx
            UInt32[] simpleRatingToRatingTable = new UInt32[] { 0, 1, 25, 50, 75, 99 };
            metadataWriter.SetMetadata("System.SimpleRating", (UInt32)value);
            metadataWriter.SetMetadata("System.Rating", simpleRatingToRatingTable[value]);
        }

        object IReadonlyMetadataProperty.Read(IMetadataReader metadataReader)
        {
            return Read(metadataReader);
        }

        void IMetadataProperty.Write(IMetadataWriter metadataWriter, object value)
        {
            Write(metadataWriter, (int)value);
        }

    }
}
