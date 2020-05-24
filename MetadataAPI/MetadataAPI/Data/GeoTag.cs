using System;
using System.Collections.Generic;
using System.Text;
using DotNetToolkit.Foundation;

namespace MetadataAPI.Data
{
    public class GeoTag
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Altitude { get; set; }

        public AltitudeReference AltitudeReference { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as GeoTag);
        }

        public bool Equals(GeoTag other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Latitude == other.Latitude
                && Longitude == other.Longitude
                && Altitude == other.Altitude
                && AltitudeReference == other.AltitudeReference;
        }

        public override int GetHashCode()
        {
            return HashCode.Of(Latitude, Longitude, Altitude, AltitudeReference);
        }

        public static bool operator ==(GeoTag obj1, GeoTag obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(GeoTag obj1, GeoTag obj2)
        {
            return !Equals(obj1, obj2);
        }
    }
}
