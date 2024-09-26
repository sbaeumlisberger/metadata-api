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
        string filePath = TestDataProvider.GetFile("Test.jpg");

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            var metadataReader = new MetadataDecoder(stream);

            Assert.Equal("Biene auf Blume", metadataReader.GetProperty(MetadataProperties.Title));

            var address = metadataReader.GetProperty(MetadataProperties.Address);
            Assert.NotNull(address);
            Assert.Equal("Pfitznerweg 41", address.Sublocation);
            Assert.Equal("74523 Schwäbisch Hall", address.City);
            Assert.Equal("BW", address.ProvinceState);
            Assert.Equal("Deutschland", address.Country);

            var geoTag = metadataReader.GetProperty(GeoTagMetadataProperty.Instance);
            Assert.NotNull(geoTag);
            Assert.Equal(49.0992, geoTag.Latitude, 4);
            Assert.Equal(9.7325, geoTag.Longitude, 4);

            var people = metadataReader.GetProperty(PeopleMetadataProperty.Instance);
            Assert.Single(people);
            Assert.Contains(people, peopleTag => peopleTag.Name == "Die Biene");

            var keywords = metadataReader.GetProperty(KeywordsMetadataProperty.Instance);
            Assert.Equal(2, keywords.Length);
            Assert.Contains("Natur/Tiere/Gliederfüßer/Insekten/Bienen", keywords);
            Assert.Contains("Natur/Pflanzen/Blumen", keywords);

            Assert.Equal(3, metadataReader.GetProperty(RatingMetadataProperty.Instance));

            var authors = metadataReader.GetProperty(AuthorMetadataProperty.Instance);
            Assert.Single(authors);
            Assert.Contains("Sebastian Bäumlisberger", authors);

            Assert.Equal("Sebastian Bäumlisberger", metadataReader.GetProperty(CopyrightMetadataProperty.Instance));

            Assert.Equal(new DateTime(2013, 6, 30, 15, 51, 13), metadataReader.GetProperty(DateTakenMetadataProperty.Instance));

            //Assert.Equal((UInt32)4912, metadataReader.GetMetadata(HorizontalSize));
            //Assert.Equal((UInt32)3264, metadataReader.GetMetadata(VerticalSize));

            Assert.Equal("SONY", metadataReader.GetProperty(CameraManufacturerMetadataProperty.Instance));
            Assert.Equal("SLT-A57", metadataReader.GetProperty(CameraModelMetadataProperty.Instance));

            Assert.Equal(new Fraction(56, 10), metadataReader.GetProperty(FNumberMetadataProperty.Instance)!.Value);
            Assert.Equal((UInt16)800, metadataReader.GetProperty(ISOSpeedMetadataProperty.Instance));
            Assert.Equal((double)55, metadataReader.GetProperty(FocalLengthMetadataProperty.Instance));
            Assert.Equal((UInt16)82, metadataReader.GetProperty(FocalLengthInFilmMetadataProperty.Instance));

            Assert.Equal(new Fraction(1, 1000), metadataReader.GetProperty(ExposureTimeMetadataProperty.Instance)!.Value.GetReduced());
            Assert.Equal(PhotoOrientation.Normal, metadataReader.GetProperty(OrientationMetadataProperty.Instance));
        }
    }
}
