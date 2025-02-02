using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class TitleMetadataPropertyIT
{

    [Theory]
    [InlineData("TestImage_metadata.jpg")]
    public void Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        Assert.Equal("Test Title", TestUtil.ReadMetadataProperty(filePath, TitleMetadataProperty.Instance));
    }

    [Theory]
    [InlineData("TestImage.jpg")]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Write(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, TitleMetadataProperty.Instance, "New Title");

        Assert.Equal("New Title", TestUtil.ReadMetadataProperty(filePath, TitleMetadataProperty.Instance));
    }

}
