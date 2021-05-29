using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIC;

namespace MetadataAPI.Properties
{
    public class RatingMetadataProperty : MetadataPropertyBase<int>
    {
        public static RatingMetadataProperty Instance { get; } = new RatingMetadataProperty();

        public override string Identifier { get; } = nameof(RatingMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private RatingMetadataProperty() { }

        public override int Read(IMetadataReader metadataReader)
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

        public override void Write(IMetadataWriter metadataWriter, int value)
        {
            if (value != 0)
            {
                if (value < 0 || value > 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be in the range 0 to 5.");
                }
                // https://msdn.microsoft.com/en-us/library/windows/desktop/bb787554(v=vs.85).aspx
                UInt32[] simpleRatingToRatingTable = new UInt32[] { 0, 1, 25, 50, 75, 99 };
                metadataWriter.SetMetadata("System.SimpleRating", (UInt32)value);
                metadataWriter.SetMetadata("System.Rating", simpleRatingToRatingTable[value]);               
            }
            else
            {
                metadataWriter.SetMetadata("System.SimpleRating", null);
                metadataWriter.SetMetadata("System.Rating", null);
            }
        }

    }
}
