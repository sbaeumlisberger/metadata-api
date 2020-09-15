using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties
{
    public class DateTakenMetadataPropertyIT
    {
        [Theory]
        [InlineData(TestConstants.JpegWithMetadata)]
        [InlineData(TestConstants.HeicWithMetadata)]
        public async Task Test_Read(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            Assert.Equal(new DateTime(2020, 4, 18, 16, 46, 51), await TestUtil.ReadMetadataPropertyAync(filePath, DateTakenMetadataProperty.Instance));
        }

        [Theory]
        [InlineData(TestConstants.JpegWithoutMetadata)]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Write(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            var dateTaken = new DateTime(1987, 12, 17, 8, 32, 7);

            await TestUtil.WriteMetadataPropertyAync(filePath, DateTakenMetadataProperty.Instance, dateTaken);

            Assert.Equal(dateTaken, await TestUtil.ReadMetadataPropertyAync(filePath, DateTakenMetadataProperty.Instance));
        }


        [Theory]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Remove(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            await TestUtil.WriteMetadataPropertyAync(filePath, DateTakenMetadataProperty.Instance, null);

            Assert.Null(await TestUtil.ReadMetadataPropertyAync(filePath, DateTakenMetadataProperty.Instance));
        }
    }
}
