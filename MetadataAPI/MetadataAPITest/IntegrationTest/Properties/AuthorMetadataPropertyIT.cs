using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties
{
    public class AuthorMetadataPropertyIT
    {
        [Theory]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Read(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            Assert.Equal(new[] { "Test Author" }, await TestUtil.ReadMetadataPropertyAync(filePath, AuthorMetadataProperty.Instance));
        }

        [Theory]
        [InlineData(TestConstants.JpegWithoutMetadata)]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Write(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            await TestUtil.WriteMetadataPropertyAync(filePath, AuthorMetadataProperty.Instance, new [] { "Author 01", "Author 02" });

            Assert.Equal(new[] { "Author 01", "Author 02" }, await TestUtil.ReadMetadataPropertyAync(filePath, AuthorMetadataProperty.Instance));
        }


        [Theory]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Remove(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            await TestUtil.WriteMetadataPropertyAync(filePath, AuthorMetadataProperty.Instance, null);

            Assert.Equal(new string[0], await TestUtil.ReadMetadataPropertyAync(filePath, AuthorMetadataProperty.Instance));
        }
    }
}
