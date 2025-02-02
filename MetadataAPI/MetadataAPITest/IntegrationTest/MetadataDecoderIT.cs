using System;
using System.IO;
using MetadataAPI;
using MetadataAPI.Data;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest;

public class MetadataDecoderIT
{
    [Fact]
    public void Test_ReadMetadata()
    {
        string filePath = TestDataProvider.GetFile("TestImage_metadata.jpg");

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            var metadataReader = new MetadataDecoder(stream);

            Assert.Equal("Test Title", metadataReader.GetProperty(MetadataProperties.Title));

            var address = metadataReader.GetProperty(MetadataProperties.Address);
            Assert.NotNull(address);
            Assert.Equal("Liberty Island 1", address.Sublocation);
            Assert.Equal("10004 New York", address.City);
            Assert.Equal("New York", address.ProvinceState);
            Assert.Equal("Vereinigte Staaten von Amerika", address.Country);

            var geoTag = metadataReader.GetProperty(GeoTagMetadataProperty.Instance);
            Assert.NotNull(geoTag);
            Assert.Equal(40.6899, geoTag.Latitude, 4);
            Assert.Equal(-74.0456, geoTag.Longitude, 4);

            var people = metadataReader.GetProperty(PeopleMetadataProperty.Instance);
            Assert.Single(people);
            Assert.Contains(people, peopleTag => peopleTag.Name == "Test");

            var keywords = metadataReader.GetProperty(KeywordsMetadataProperty.Instance);
            Assert.Equal(3, keywords.Length);
            Assert.Contains("Test/Test 01", keywords);
            Assert.Contains("Test/Test 02", keywords);
            Assert.Contains("Test/Test 03", keywords);

            Assert.Equal(3, metadataReader.GetProperty(RatingMetadataProperty.Instance));

            var authors = metadataReader.GetProperty(AuthorMetadataProperty.Instance);
            Assert.Single(authors);
            Assert.Contains("Test Author", authors);

            Assert.Equal("Test Copyright", metadataReader.GetProperty(CopyrightMetadataProperty.Instance));

            Assert.Equal(new DateTime(2020, 4, 18, 16, 46, 51), metadataReader.GetProperty(DateTakenMetadataProperty.Instance));

            //Assert.Equal((UInt32)4912, metadataReader.GetMetadata(HorizontalSize));
            //Assert.Equal((UInt32)3264, metadataReader.GetMetadata(VerticalSize));

            Assert.Equal("SONY", metadataReader.GetProperty(CameraManufacturerMetadataProperty.Instance));
            Assert.Equal("SLT-A57", metadataReader.GetProperty(CameraModelMetadataProperty.Instance));

            Assert.Equal(new Fraction(56, 10), metadataReader.GetProperty(FNumberMetadataProperty.Instance)!.Value);
            Assert.Equal((UInt16)800, metadataReader.GetProperty(ISOSpeedMetadataProperty.Instance));
            Assert.Equal((double)55, metadataReader.GetProperty(FocalLengthMetadataProperty.Instance));
            Assert.Equal((UInt16)82, metadataReader.GetProperty(FocalLengthInFilmMetadataProperty.Instance));
            Assert.Equal(new Fraction(1, 1000), metadataReader.GetProperty(ExposureTimeMetadataProperty.Instance)!.Value.GetReduced());
            
            Assert.Equal(PhotoOrientation.Unspecified, metadataReader.GetProperty(OrientationMetadataProperty.Instance));
        }
    }
}
