using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI;
using WIC;
using Xunit;

namespace MetadataAPITest.IntegrationTest
{
    public class MetadataEncoderIT
    {
        private const string FileWithPadding = "TestImage_with_padding.jpg";
        private const string FileWithoutPadding = "TestImage_without_padding.jpg";

        [Fact]
        public async Task Test_EncodeInPlace()
        {
            string filePath = TestDataProvider.GetFile(FileWithPadding);

            byte[] pixelsBefore = GetPixels(filePath);
            long sizeBefore = GetFileSize(filePath);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataEncoder = new MetadataEncoder(stream);
                metadataEncoder.SetProperty(MetadataProperties.Title, "New Title");
                await metadataEncoder.EncodeAsync();
            }

            Assert.Equal("New Title", await TestUtil.ReadMetadataPropertyAync(filePath, MetadataProperties.Title));
            Assert.Equal(pixelsBefore, GetPixels(filePath));
            Assert.Equal(sizeBefore, GetFileSize(filePath));
        }

        [Theory]
        [InlineData(FileWithoutPadding, 0)]
        [InlineData(FileWithoutPadding, 2048)]
        [InlineData(FileWithoutPadding, 4096)]
        public async Task Test_ReEncode(string fileName, uint padding)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            byte[] pixelsBefore = GetPixels(filePath);
            long sizeBefore = GetFileSize(filePath);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataEncoder = new MetadataEncoder(stream);
                metadataEncoder.PaddingAmount = padding;
                metadataEncoder.SetProperty(MetadataProperties.Title, "New Title");
                await metadataEncoder.EncodeAsync();
            }

            Assert.Equal("New Title", await TestUtil.ReadMetadataPropertyAync(filePath, MetadataProperties.Title));
            Assert.Equal(pixelsBefore, GetPixels(filePath));
            AssertUtil.GreaterThan(sizeBefore, GetFileSize(filePath));
            await AssertPaddingAsync(padding, filePath).ConfigureAwait(false);
        }

        private async Task AssertPaddingAsync(uint padding, string filePath)
        {
            long sizeBefore = GetFileSize(filePath);
            
            await EncodeAsync(filePath, new MetadataPropertySet() {
               { MetadataProperties.Title, "New longer title" }
            }).ConfigureAwait(false);

            if (padding > 0)
            {
                Assert.Equal(sizeBefore, GetFileSize(filePath));
            }
            else 
            {
                AssertUtil.GreaterThan(sizeBefore, GetFileSize(filePath));
            }
        }

        private async Task EncodeAsync(string filePath, MetadataPropertySet propertySet)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataEncoder = new MetadataEncoder(stream);
                metadataEncoder.SetProperties(propertySet);
                await metadataEncoder.EncodeAsync().ConfigureAwait(false);
            }
        }

        private byte[] GetPixels(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var wic = new WICImagingFactory();
                var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);
                return decoder.GetFrame(0).GetPixels();
            }
        }

        private long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

    }
}
