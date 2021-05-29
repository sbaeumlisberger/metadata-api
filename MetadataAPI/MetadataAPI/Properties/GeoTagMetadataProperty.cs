using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using MetadataAPI.Data;
using MetadataAPI.Utils;
using WIC;

namespace MetadataAPI.Properties
{
    public class GeoTagMetadataProperty : MetadataPropertyBase<GeoTag?>
    {
        public static GeoTagMetadataProperty Instance { get; } = new GeoTagMetadataProperty();

        public override string Identifier { get; } = nameof(GeoTagMetadataProperty);

        public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg, ContainerFormat.Tiff };

        private GeoTagMetadataProperty() { }

        public override GeoTag? Read(IMetadataReader metadataReader)
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
                    Altitude = altitude,
                    AltitudeReference = ((AltitudeReference?)altitudeReference) ?? AltitudeReference.Unspecified
                };
            }

            return null;
        }

        public override void Write(IMetadataWriter metadataWriter, GeoTag? geoTag)
        {
            if (geoTag is null)
            {
                RemoveGPSMetadata(metadataWriter);
            }
            else
            {
                /* Remove already existing GPS metadata, because
                 * overwriting sometimes doesn't work properly. */
                RemoveGPSMetadata(metadataWriter);

                SetLatitude(metadataWriter, geoTag);
                SetLongitude(metadataWriter, geoTag);
                SetAltitude(metadataWriter, geoTag);
            }
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

                double decimalValue = GeoUtil.DegreesMinutesSecondsToDecimal((uint)degree, (uint)minutes, seconds);

                if (metadataReader.GetMetadata(refKey) is string latitudeRef
                    && latitudeRef.Equals(refNegative, StringComparison.OrdinalIgnoreCase))
                {
                    return decimalValue * -1;
                }
                return decimalValue;
            }
            return null;
        }

        private void RemoveGPSMetadata(IMetadataWriter metadataWriter)
        {
            metadataWriter.SetMetadata("System.GPS.LatitudeNumerator", null);
            metadataWriter.SetMetadata("System.GPS.LatitudeDenominator", null);
            metadataWriter.SetMetadata("System.GPS.LatitudeRef", null);
            metadataWriter.SetMetadata("System.GPS.LongitudeNumerator", null);
            metadataWriter.SetMetadata("System.GPS.LongitudeDenominator", null);
            metadataWriter.SetMetadata("System.GPS.LongitudeRef", null);
            metadataWriter.SetMetadata("System.GPS.AltitudeNumerator", null);
            metadataWriter.SetMetadata("System.GPS.AltitudeDenominator", null);
            metadataWriter.SetMetadata("System.GPS.AltitudeRef", null);
        }

        private void SetLatitude(IMetadataWriter metadataWriter, GeoTag geoTag)
        {
            var (degree, minutes, seconds) = GeoUtil.DecimalToDegreesMinutesSeconds(geoTag.Latitude);

            Fraction degreesFraction = new Fraction(degree, 1);
            Fraction minutesFraction = new Fraction(minutes, 1);
            Fraction secondsFraction = new Fraction((long)Math.Round(seconds * Math.Pow(10, 7)), (long)Math.Pow(10, 7)).GetReduced();

            metadataWriter.SetMetadata("System.GPS.LatitudeNumerator", new uint[]
            {
                    (uint)degreesFraction.Numerator,
                    (uint)minutesFraction.Numerator,
                    (uint)secondsFraction.Numerator
            });
            metadataWriter.SetMetadata("System.GPS.LatitudeDenominator", new uint[]
            {
                    (uint)degreesFraction.Denominator,
                    (uint)minutesFraction.Denominator,
                    (uint)secondsFraction.Denominator
            });
            metadataWriter.SetMetadata("System.GPS.LatitudeRef", geoTag.Latitude < 0 ? "S" : "N");
        }

        private void SetLongitude(IMetadataWriter metadataWriter, GeoTag geoTag)
        {
            var (degree, minutes, seconds) = GeoUtil.DecimalToDegreesMinutesSeconds(geoTag.Longitude);

            Fraction degreesFraction = new Fraction(degree, 1);
            Fraction minutesFraction = new Fraction(minutes, 1);
            Fraction secondsFraction = new Fraction((long)Math.Round(seconds * Math.Pow(10, 7)), (long)Math.Pow(10, 7)).GetReduced();

            metadataWriter.SetMetadata("System.GPS.LongitudeNumerator", new uint[]
            {
                    (uint)degreesFraction.Numerator,
                    (uint)minutesFraction.Numerator,
                    (uint)secondsFraction.Numerator
            });
            metadataWriter.SetMetadata("System.GPS.LongitudeDenominator", new uint[]
            {
                    (uint)degreesFraction.Denominator,
                    (uint)minutesFraction.Denominator,
                    (uint)secondsFraction.Denominator
            });
            metadataWriter.SetMetadata("System.GPS.LongitudeRef", geoTag.Longitude < 0 ? "W" : "E");
        }

        private void SetAltitude(IMetadataWriter metadataWriter, GeoTag geoTag)
        {
            if (geoTag.Altitude is double altitudeDecimal)
            {
                if (altitudeDecimal < 0)
                {
                    throw new ArgumentException("Altitude must be greater than or equal to 0."); // TODO ?
                }

                Fraction altitude = new Fraction((long)Math.Round(altitudeDecimal * Math.Pow(10, 4)), (long)Math.Pow(10, 4));
                byte altitudeRef = (byte)geoTag.AltitudeReference;

                metadataWriter.SetMetadata("System.GPS.AltitudeNumerator", (uint)altitude.Numerator);
                metadataWriter.SetMetadata("System.GPS.AltitudeDenominator", (uint)altitude.Denominator);
                metadataWriter.SetMetadata("System.GPS.AltitudeRef", altitudeRef);
            }
            else
            {
                metadataWriter.SetMetadata("System.GPS.AltitudeNumerator", null);
                metadataWriter.SetMetadata("System.GPS.AltitudeDenominator", null);
                metadataWriter.SetMetadata("System.GPS.AltitudeRef", null);
            }
        }

    }
}
