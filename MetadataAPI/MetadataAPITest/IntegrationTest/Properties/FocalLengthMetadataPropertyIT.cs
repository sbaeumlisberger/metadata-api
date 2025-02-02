using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class FocalLengthMetadataPropertyIT
{

    [Theory]
    [InlineData("TestImage_metadata.jpg")]
    public void Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        Assert.Equal(55, TestUtil.ReadMetadataProperty(filePath, FocalLengthMetadataProperty.Instance));
    }

    [Theory]
    [InlineData("TestImage.jpg")]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Write(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, FocalLengthMetadataProperty.Instance, 90);

        Assert.Equal(90, TestUtil.ReadMetadataProperty(filePath, FocalLengthMetadataProperty.Instance));
    }

}
