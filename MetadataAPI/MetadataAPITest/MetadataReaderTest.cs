using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetToolkit.Foundation;
using MetadataAPI;
using MetadataAPI.Data;
using MetadataAPI.Definitions;
using Xunit;

namespace MetadataAPITest
{
    public class MetadataReaderTest
    {
        [Fact]
        public void Test_ReadMetadata()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "/Test.jpg";

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataReader = new MetadataReader(stream, Path.GetExtension(filePath));

                Assert.Equal("Biene auf Blume", metadataReader.GetMetadata(TitleMetadataProperty.Instance));

                Assert.Equal("Pfitznerweg 41", metadataReader.GetMetadata(AddressMetadataProperty.Instance).Sublocation);
                Assert.Equal("74523 Schwäbisch Hall", metadataReader.GetMetadata(AddressMetadataProperty.Instance).City);
                Assert.Equal("BW", metadataReader.GetMetadata(AddressMetadataProperty.Instance).ProvinceState);
                Assert.Equal("Deutschland", metadataReader.GetMetadata(AddressMetadataProperty.Instance).Country);

                Assert.Equal(49.0992, metadataReader.GetMetadata(GeoTagMetadataProperty.Instance).Latitude, 4);
                Assert.Equal(9.7325, metadataReader.GetMetadata(GeoTagMetadataProperty.Instance).Longitude, 4);

                var people = metadataReader.GetMetadata(PeopleMetadataProperty.Instance);
                Assert.Equal(1, people.Count);
                Assert.Contains(people, peopleTag => peopleTag.Name == "Die Biene");

                var keywords = metadataReader.GetMetadata(KeywordsMetadataProperty.Instance);
                Assert.Equal(2, keywords.Length);
                Assert.Contains("Natur/Tiere/Gliederfüßer/Insekten/Bienen", keywords);
                Assert.Contains("Natur/Pflanzen/Blumen", keywords);

                Assert.Equal(3, metadataReader.GetMetadata(RatingMetadataProperty.Instance));

                var authors = metadataReader.GetMetadata(AuthorMetadataProperty.Instance);
                Assert.Equal(1, authors.Length);
                Assert.Contains("Sebastian Bäumlisberger", metadataReader.GetMetadata(AuthorMetadataProperty.Instance));

                Assert.Equal("Sebastian Bäumlisberger", metadataReader.GetMetadata(CopyrightMetadataProperty.Instance));

                Assert.Equal(new DateTime(2013, 6, 30, 15, 51, 13), metadataReader.GetMetadata(DateTakenMetadataProperty.Instance));

                //Assert.Equal((UInt32)4912, metadataReader.GetMetadata(HorizontalSize));
                //Assert.Equal((UInt32)3264, metadataReader.GetMetadata(VerticalSize));

                Assert.Equal("SONY", metadataReader.GetMetadata(CameraManufacturerMetadataProperty.Instance));
                Assert.Equal("SLT-A57", metadataReader.GetMetadata(CameraModelMetadataProperty.Instance));

                Assert.Equal(new Fraction(56, 10), metadataReader.GetMetadata(FNumberMetadataProperty.Instance).Value);
                Assert.Equal((UInt16)800, metadataReader.GetMetadata(ISOSpeedMetadataProperty.Instance));
                Assert.Equal((double)55, metadataReader.GetMetadata(FocalLengthMetadataProperty.Instance));
                Assert.Equal((UInt16)82, metadataReader.GetMetadata(FocalLengthInFilmMetadataProperty.Instance));

                Assert.Equal(new Fraction(1, 1000), metadataReader.GetMetadata(ExposureTimeMetadataProperty.Instance).Value.GetReduced());
                Assert.Equal(PhotoOrientation.Normal, metadataReader.GetMetadata(OrientationMetadataProperty.Instance));
            }
        }
    }
}
