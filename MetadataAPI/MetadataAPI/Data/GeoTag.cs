namespace MetadataAPI.Data;

public record class GeoTag
{
    /// <summary>
    /// Latitude in decimal degrees.
    /// </summary>
    public required double Latitude { get; init; }

    /// <summary>
    /// Longitude in decimal degrees.
    /// </summary>
    public required double Longitude { get; init; }

    /// <summary>
    /// Altitude in meters.
    /// </summary>
    public double? Altitude { get; init; }

    public GeoTag() { }

    public GeoTag(double latitude, double longitude, double? altitude = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
    }
}
