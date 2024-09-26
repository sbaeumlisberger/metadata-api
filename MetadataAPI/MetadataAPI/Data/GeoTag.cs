namespace MetadataAPI.Data;

public record class GeoTag
{
    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public double? Altitude { get; init; }

    public AltitudeReference AltitudeReference { get; init; }
}
