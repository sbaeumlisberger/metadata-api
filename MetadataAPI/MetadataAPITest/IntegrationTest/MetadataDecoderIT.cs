using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetToolkit.Foundation;
using MetadataAPI;
using MetadataAPI.Data;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest
{
    public class MetadataDecoderIT
    {
        [Fact]
        public void Test_ReadMetadata()
        {
            string filePath = TestDataProvider.GetFile("Test.jpg");

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataReader = new MetadataDecoder(stream, Path.GetExtension(filePath));

                Assert.Equal("Biene auf Blume", metadataReader.GetProperty(MetadataProperties.Title));

                Assert.Equal("Pfitznerweg 41", metadataReader.GetProperty(MetadataProperties.Address).Sublocation);
                Assert.Equal("74523 Schwäbisch Hall", metadataReader.GetProperty(MetadataProperties.Address).City);
                Assert.Equal("BW", metadataReader.GetProperty(MetadataProperties.Address).ProvinceState);
                Assert.Equal("Deutschland", metadataReader.GetProperty(MetadataProperties.Address).Country);

                Assert.Equal(49.0992, metadataReader.GetProperty(GeoTagMetadataProperty.Instance).Latitude, 4);
                Assert.Equal(9.7325, metadataReader.GetProperty(GeoTagMetadataProperty.Instance).Longitude, 4);

                var people = metadataReader.GetProperty(PeopleMetadataProperty.Instance);
                Assert.Equal(1, people.Count);
                Assert.Contains(people, peopleTag => peopleTag.Name == "Die Biene");

                var keywords = metadataReader.GetProperty(KeywordsMetadataProperty.Instance);
                Assert.Equal(2, keywords.Length);
                Assert.Contains("Natur/Tiere/Gliederfüßer/Insekten/Bienen", keywords);
                Assert.Contains("Natur/Pflanzen/Blumen", keywords);

                Assert.Equal(3, metadataReader.GetProperty(RatingMetadataProperty.Instance));

                var authors = metadataReader.GetProperty(AuthorMetadataProperty.Instance);
                Assert.Equal(1, authors.Length);
                Assert.Contains("Sebastian Bäumlisberger", authors);

                Assert.Equal("Sebastian Bäumlisberger", metadataReader.GetProperty(CopyrightMetadataProperty.Instance));

                Assert.Equal(new DateTime(2013, 6, 30, 15, 51, 13), metadataReader.GetProperty(DateTakenMetadataProperty.Instance));

                //Assert.Equal((UInt32)4912, metadataReader.GetMetadata(HorizontalSize));
                //Assert.Equal((UInt32)3264, metadataReader.GetMetadata(VerticalSize));

                Assert.Equal("SONY", metadataReader.GetProperty(CameraManufacturerMetadataProperty.Instance));
                Assert.Equal("SLT-A57", metadataReader.GetProperty(CameraModelMetadataProperty.Instance));

                Assert.Equal(new Fraction(56, 10), metadataReader.GetProperty(FNumberMetadataProperty.Instance).Value);
                Assert.Equal((UInt16)800, metadataReader.GetProperty(ISOSpeedMetadataProperty.Instance));
                Assert.Equal((double)55, metadataReader.GetProperty(FocalLengthMetadataProperty.Instance));
                Assert.Equal((UInt16)82, metadataReader.GetProperty(FocalLengthInFilmMetadataProperty.Instance));

                Assert.Equal(new Fraction(1, 1000), metadataReader.GetProperty(ExposureTimeMetadataProperty.Instance).Value.GetReduced());
                Assert.Equal(PhotoOrientation.Normal, metadataReader.GetProperty(OrientationMetadataProperty.Instance));
            }
        }
    }
}
