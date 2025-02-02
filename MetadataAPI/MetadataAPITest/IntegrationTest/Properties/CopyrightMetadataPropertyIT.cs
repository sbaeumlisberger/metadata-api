using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class CopyrightMetadataPropertyIT
{
    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    public void Test_Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        Assert.Equal("Test Copyright", TestUtil.ReadMetadataProperty(filePath, CopyrightMetadataProperty.Instance));
    }

    [Theory]
    [InlineData(TestConstants.JpegWithoutMetadata)]
    [InlineData(TestConstants.JpegWithMetadata)]
    public async Task Test_Write(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, CopyrightMetadataProperty.Instance, "New Copyright");

        Assert.Equal("New Copyright", TestUtil.ReadMetadataProperty(filePath, CopyrightMetadataProperty.Instance));
    }

    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    public async Task Test_Remove(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, CopyrightMetadataProperty.Instance, "");

        Assert.Empty(TestUtil.ReadMetadataProperty(filePath, CopyrightMetadataProperty.Instance));
    }
}
