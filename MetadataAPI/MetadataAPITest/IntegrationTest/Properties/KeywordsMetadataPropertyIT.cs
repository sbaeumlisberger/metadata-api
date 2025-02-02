using System;
using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class KeywordsMetadataPropertyIT
{
    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    public void Test_Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        Assert.Equal(new[] { "Test/Test 01", "Test/Test 02", "Test/Test 03" }, TestUtil.ReadMetadataProperty(filePath, KeywordsMetadataProperty.Instance));
    }

    [Theory]
    [InlineData(TestConstants.JpegWithoutMetadata)]
    [InlineData(TestConstants.JpegWithMetadata)]
    public async Task Test_Write(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var keywords = new[] { "Test/New 01", "New 02", "New 03", "Test/New 04" };

        await TestUtil.WriteMetadataPropertyAync(filePath, KeywordsMetadataProperty.Instance, keywords);

        Assert.Equal(keywords, TestUtil.ReadMetadataProperty(filePath, KeywordsMetadataProperty.Instance));
    }

    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    public async Task Test_Write_LessThanBefore(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var keywords = new[] { "Test/New 01" };

        await TestUtil.WriteMetadataPropertyAync(filePath, KeywordsMetadataProperty.Instance, keywords);

        Assert.Equal(keywords, TestUtil.ReadMetadataProperty(filePath, KeywordsMetadataProperty.Instance));
    }

    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    public async Task Test_Remove(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, KeywordsMetadataProperty.Instance, Array.Empty<string>());

        Assert.Empty(TestUtil.ReadMetadataProperty(filePath, KeywordsMetadataProperty.Instance));
    }
}
