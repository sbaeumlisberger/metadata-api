using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI.Data
{
    public class GeoTag
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public AltitudeReference AltitudeReference { get; set; }
    }
}
