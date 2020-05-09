using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI.Data
{
    public enum AltitudeReference
    {
        Unspecified = 0,
        Terrain = 1,
        Ellipsoid = 2,
        Geoid = 3,
        Surface = 4
    }
}
