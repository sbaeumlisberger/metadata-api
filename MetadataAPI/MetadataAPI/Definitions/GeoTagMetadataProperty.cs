using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataAPI.Data;

namespace MetadataAPI.Definitions
{
    public class GeoTagMetadataProperty : IMetadataProperty<GeoTag>
    {
        public string Identifier { get; } = nameof(GeoTagMetadataProperty);

        public IReadOnlyCollection<string> SupportedFileTypes { get; } = new HashSet<string>(FileTypes.JpegExtensions.Concat(FileTypes.TiffExtensions));

        public GeoTag Read(IMetadataReader metadataReader)
        {
            double? latitude = GetDecimal(metadataReader, "System.GPS.LatitudeNumerator", "System.GPS.LatitudeDenominator", "System.GPS.LatitudeRef", "S");
            double? longitude = GetDecimal(metadataReader, "System.GPS.LongitudeNumerator", "System.GPS.LongitudeDenominator", "System.GPS.LongitudeRef", "W");

            if (latitude != null && longitude != null)
            {
                double? altitude = GetAltitude(metadataReader);
                byte? altitudeReference = (byte?)metadataReader.GetMetadata("System.GPS.AltitudeRef");

                return new GeoTag()
                {
                    Latitude = (double)latitude,
                    Longitude = (double)longitude,
                    Altitude = altitude ?? 0,
                    AltitudeReference = altitudeReference != null
                        ? (AltitudeReference)altitudeReference
                        : AltitudeReference.Unspecified
                };
            }

            return null;
        }

        public void Write(IMetadataWriter metadataWriter, GeoTag value)
        {
            metadataWriter.SetMetadata("System.Copyright", value);
        }

        private double? GetAltitude(IMetadataReader metadataReader)
        {
            if (metadataReader.GetMetadata("System.GPS.AltitudeNumerator") is UInt32 numerator
                && metadataReader.GetMetadata("System.GPS.AltitudeDenominator") is UInt32 denominator)
            {
                return ((double)numerator) / denominator;
            }
            return null;
        }

        private double? GetDecimal(IMetadataReader metadataReader, string numeratorKey, string denominatorKey, string refKey, string refNegative)
        {
            if (metadataReader.GetMetadata(numeratorKey) is UInt32[] numerator
                && metadataReader.GetMetadata(denominatorKey) is UInt32[] denominator)
            {
                double degree = (double)numerator[0] / denominator[0];
                double minutes = (double)numerator[1] / denominator[1];
                double seconds = (double)numerator[2] / denominator[2];

                double decimalValue = ((seconds / 60) + minutes) / 60 + degree;

                if (metadataReader.GetMetadata(refKey) is string latitudeRef
                    && latitudeRef.Equals(refNegative, StringComparison.OrdinalIgnoreCase))
                {
                    return decimalValue * -1;
                }
                return decimalValue;
            }
            return null;
        }
    }
}
