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

    [Fact]
    public async Task Test_EncodeInPlace()
    {
        string filePath = TestDataProvider.GetFile(TestImageWithPadding);

        byte[] pixelsBefore = GetPixels(filePath);
        long sizeBefore = GetFileSize(filePath);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            var metadataEncoder = new MetadataEncoder(stream);
            metadataEncoder.SetProperty(MetadataProperties.Title, "New Title");
            await metadataEncoder.EncodeAsync();
        }

        Assert.Equal("New Title", await TestUtil.ReadMetadataPropertyAync(filePath, MetadataProperties.Title));
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
            metadataEncoder.SetProperty(MetadataProperties.Title, "New Title");
            await metadataEncoder.EncodeAsync();
        }

        Assert.Equal("New Title", await TestUtil.ReadMetadataPropertyAync(filePath, MetadataProperties.Title));
        Assert.True(pixelsBefore.SequenceEqual(GetPixels(filePath)));
        AssertUtil.GreaterThan(sizeBefore, GetFileSize(filePath));
        await AssertPaddingAsync(padding, filePath);
    }

    [Theory]
    [InlineData(TestImageWithLargeThumbnail, 0)]
    [InlineData(TestImageWithLargeThumbnail, 2048)]
    [InlineData(TestImageWithLargeThumbnail, 4096)]
    public async Task Test_ErrorToMuchMetadata(string fileName, uint padding)
    {
        string filePath = TestDataProvider.GetFile(fileName, "_" + padding.ToString());

        var exception = await Assert.ThrowsAsync<COMException>(async () =>
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataEncoder = new MetadataEncoder(stream);
                metadataEncoder.PaddingAmount = padding;
                metadataEncoder.SetProperty(MetadataProperties.Title, "New Title");
                await metadataEncoder.EncodeAsync();
            }
        });

        Assert.Equal(WinCodecError.TOO_MUCH_METADATA, exception.HResult);
    }

    [Theory]
    [InlineData(TestImageWithLargeThumbnail, 0)]
    [InlineData(TestImageWithLargeThumbnail, 2048)]
    [InlineData(TestImageWithLargeThumbnail, 4096)]
    public async Task Test_AutoResizeLargeThumbnails(string fileName, uint padding)
    {
        string filePath = TestDataProvider.GetFile(fileName, "_" + padding.ToString());

        byte[] pixelsBefore = GetPixels(filePath);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            var metadataEncoder = new MetadataEncoder(stream);
            metadataEncoder.AutoResizeLargeThumbnails = true;
            metadataEncoder.PaddingAmount = padding;
            metadataEncoder.SetProperty(MetadataProperties.Title, "New Title");
            await metadataEncoder.EncodeAsync();
        }

        Assert.Equal("New Title", await TestUtil.ReadMetadataPropertyAync(filePath, MetadataProperties.Title));
        Assert.True(pixelsBefore.SequenceEqual(GetPixels(filePath)));
        await AssertPaddingAsync(padding, filePath);
    }

    private async Task AssertPaddingAsync(uint padding, string filePath)
    {
        long sizeBefore = GetFileSize(filePath);

        await EncodeAsync(filePath, new MetadataPropertySet() {
           { MetadataProperties.Title, "New longer title" }
        });

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
            await metadataEncoder.EncodeAsync();
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
