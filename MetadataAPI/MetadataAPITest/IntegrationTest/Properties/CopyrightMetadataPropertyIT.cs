using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties
{
    public class CopyrightMetadataPropertyIT
    {
        [Theory]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Read(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            Assert.Equal("Test Copyright", await TestUtil.ReadMetadataPropertyAync(filePath, CopyrightMetadataProperty.Instance));
        }

        [Theory]
        [InlineData(TestConstants.JpegWithoutMetadata)]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Write(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            await TestUtil.WriteMetadataPropertyAync(filePath, CopyrightMetadataProperty.Instance, "New Copyright");

            Assert.Equal("New Copyright", await TestUtil.ReadMetadataPropertyAync(filePath, CopyrightMetadataProperty.Instance));
        }

        [Theory]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Remove(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            await TestUtil.WriteMetadataPropertyAync(filePath, CopyrightMetadataProperty.Instance, null);

            Assert.Equal(string.Empty, await TestUtil.ReadMetadataPropertyAync(filePath, CopyrightMetadataProperty.Instance));
        }
    }
}
