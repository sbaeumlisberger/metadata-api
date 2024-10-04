using System.Threading.Tasks;
using MetadataAPI;
using MetadataAPI.Data;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class GeoTagMetadataPropertyIT
{

    [Theory]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var geoTag = await TestUtil.ReadMetadataPropertyAync(filePath, GeoTagMetadataProperty.Instance);

        Assert.NotNull(geoTag);
        Assert.Equal(40.6899245, geoTag.Latitude, 6);
        Assert.Equal(-74.0455788, geoTag.Longitude, 6);
        Assert.NotNull(geoTag.Altitude);
        Assert.Equal(12.3871, (double)geoTag.Altitude, 3);
    }

    [Theory]
    [InlineData("TestImage.jpg")]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Write_WithoutAltitude(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var geoTag = new GeoTag()
        {
            Latitude = 33.7601823,
            Longitude = -118.1941221
        };

        await TestUtil.WriteMetadataPropertyAync(filePath, GeoTagMetadataProperty.Instance, geoTag);

        var readGeoTag = await TestUtil.ReadMetadataPropertyAync(filePath, GeoTagMetadataProperty.Instance);

        Assert.NotNull(readGeoTag);
        Assert.Equal(geoTag.Latitude, readGeoTag.Latitude, 6);
        Assert.Equal(geoTag.Longitude, readGeoTag.Longitude, 6);
        Assert.Null(readGeoTag.Altitude);
    }

    [Theory]
    [InlineData("TestImage.jpg")]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Write_WithAltitude(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var geoTag = new GeoTag()
        {
            Latitude = 52.5144077,
            Longitude = 13.3497061,
            Altitude = 73.2512
        };

        await TestUtil.WriteMetadataPropertyAync(filePath, GeoTagMetadataProperty.Instance, geoTag);

        var readGeoTag = await TestUtil.ReadMetadataPropertyAync(filePath, GeoTagMetadataProperty.Instance);

        Assert.NotNull(readGeoTag);
        Assert.Equal(geoTag.Latitude, readGeoTag.Latitude, 6);
        Assert.Equal(geoTag.Longitude, readGeoTag.Longitude, 6);
        Assert.NotNull(readGeoTag.Altitude);
        Assert.Equal((double)geoTag.Altitude, (double)readGeoTag.Altitude, 3);
    }

    [Theory]
    [InlineData("TestImage.jpg")]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Write_WithNegativeAltitude(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var geoTag = new GeoTag()
        {
            Latitude = 52.5144077,
            Longitude = 13.3497061,
            Altitude = -22.3157
        };

        await TestUtil.WriteMetadataPropertyAync(filePath, GeoTagMetadataProperty.Instance, geoTag);

        var readGeoTag = await TestUtil.ReadMetadataPropertyAync(filePath, GeoTagMetadataProperty.Instance);

        Assert.NotNull(readGeoTag);
        Assert.Equal(geoTag.Latitude, readGeoTag.Latitude, 6);
        Assert.Equal(geoTag.Longitude, readGeoTag.Longitude, 6);
        Assert.NotNull(readGeoTag.Altitude);
        Assert.Equal((double)geoTag.Altitude, (double)readGeoTag.Altitude, 3);
    }

}
