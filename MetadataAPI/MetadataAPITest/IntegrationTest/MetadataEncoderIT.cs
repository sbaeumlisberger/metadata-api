using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MetadataAPI;
using WIC;
using Xunit;

namespace MetadataAPITest.IntegrationTest;

public class MetadataEncoderIT
{
    private const string TestImageWithPadding = "TestImage_with_padding.jpg";
    private const string TestImageWithoutPadding = "TestImage_without_padding.jpg";
    private const string TestImageWithLargeThumbnail = "TestImage_LargeThumbnail.jpg";

    private const string TestTitle = "New Title";
    private const string TestLongerTitle = TestTitle + " Longer";

    [Fact]
    public async Task Test_EncodeInPlace()
    {
        string filePath = TestDataProvider.GetFile(TestImageWithPadding);

        byte[] pixelsBefore = GetPixels(filePath);
        long sizeBefore = GetFileSize(filePath);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            var metadataEncoder = new MetadataEncoder(stream);
            metadataEncoder.SetProperty(MetadataProperties.Title, TestTitle);
            await metadataEncoder.EncodeAsync();
        }

        Assert.Equal(TestTitle, TestUtil.ReadMetadataProperty(filePath, MetadataProperties.Title));
        Assert.True(pixelsBefore.SequenceEqual(GetPixels(filePath)));
        Assert.Equal(sizeBefore, GetFileSize(filePath));
    }

    [Theory]
    [InlineData(TestImageWithoutPadding, 0)]
    [InlineData(TestImageWithoutPadding, 2048)]
    [InlineData(TestImageWithoutPadding, 4096)]
    public async Task Test_ReEncode(string fileName, uint padding)
    {
        string filePath = TestDataProvider.GetFile(fileName, "_" + padding.ToString());

        byte[] pixelsBefore = GetPixels(filePath);
        long sizeBefore = GetFileSize(filePath);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            var metadataEncoder = new MetadataEncoder(stream);
            metadataEncoder.PaddingAmount = padding;
            metadataEncoder.SetProperty(MetadataProperties.Title, TestTitle);
            await metadataEncoder.EncodeAsync();
        }

        Assert.Equal(TestTitle, TestUtil.ReadMetadataProperty(filePath, MetadataProperties.Title));
        Assert.True(pixelsBefore.SequenceEqual(GetPixels(filePath)));
        AssertUtil.GreaterThan(sizeBefore, GetFileSize(filePath));
        await AssertPaddingAsync(padding, filePath);
    }

    [Fact]
    public async Task Test_ErrorToMuchMetadata()
    {
        string filePath = TestDataProvider.GetFile(TestImageWithLargeThumbnail);

        var exception = await Assert.ThrowsAsync<COMException>(async () =>
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataEncoder = new MetadataEncoder(stream);
                metadataEncoder.PaddingAmount = 2048;
                metadataEncoder.SetProperty(MetadataProperties.Title, TestTitle);
                await metadataEncoder.EncodeAsync();
            }
        });

        Assert.Equal(WinCodecError.TOO_MUCH_METADATA, exception.HResult);
    }

    [Fact]
    public async Task Test_AutoResizeLargeThumbnails()
    {
        string filePath = TestDataProvider.GetFile(TestImageWithLargeThumbnail);
        uint padding = 2048;

        byte[] pixelsBefore = GetPixels(filePath);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            var metadataEncoder = new MetadataEncoder(stream);
            metadataEncoder.AutoResizeLargeThumbnails = true;
            metadataEncoder.PaddingAmount = padding;
            metadataEncoder.SetProperty(MetadataProperties.Title, TestTitle);
            await metadataEncoder.EncodeAsync();
        }

        AssertThumbnailSize(filePath, 256, 192);
        Assert.Equal(TestTitle, TestUtil.ReadMetadataProperty(filePath, MetadataProperties.Title));
        Assert.True(pixelsBefore.SequenceEqual(GetPixels(filePath)));
        await AssertPaddingAsync(padding, filePath);
    }

    private async Task AssertPaddingAsync(uint padding, string filePath)
    {
        if (padding == 0)
        {
            return;
        }

        long sizeBefore = GetFileSize(filePath);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            var metadataEncoder = new MetadataEncoder(stream);
            metadataEncoder.SetProperty(MetadataProperties.Title, TestLongerTitle);
            await metadataEncoder.EncodeAsync();
        }

        Assert.Equal(sizeBefore, GetFileSize(filePath));
    }

    private void AssertThumbnailSize(string filePath, int expectedWidth, int expectedHeight)
    {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            var wic = WICImagingFactory.Create();
            var decoder = wic.CreateDecoderFromStream(stream, WICDecodeOptions.WICDecodeMetadataCacheOnDemand);
            var thumbnailSize = decoder.GetFrame(0).GetThumbnail().GetSize();
            Assert.Equal(expectedWidth, thumbnailSize.Width);
            Assert.Equal(expectedHeight, thumbnailSize.Height);
        }
    }

    private byte[] GetPixels(string filePath)
    {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            var wic = WICImagingFactory.Create();
            var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);
            return decoder.GetFrame(0).GetPixels();
        }
    }

    private long GetFileSize(string filePath)
    {
        return new FileInfo(filePath).Length;
    }

}
