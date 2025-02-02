using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class AuthorMetadataPropertyIT
{
    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    public void Test_Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        Assert.Equal(["Test Author"], TestUtil.ReadMetadataProperty(filePath, AuthorMetadataProperty.Instance));
    }

    [Theory]
    [InlineData(TestConstants.JpegWithoutMetadata)]
    [InlineData(TestConstants.JpegWithMetadata)]
    public async Task Test_Write(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, AuthorMetadataProperty.Instance, ["Author 01", "Author 02"]);

        Assert.Equal(["Author 01", "Author 02"], TestUtil.ReadMetadataProperty(filePath, AuthorMetadataProperty.Instance));
    }


    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    public async Task Test_Remove(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, AuthorMetadataProperty.Instance, []);

        Assert.Empty(TestUtil.ReadMetadataProperty(filePath, AuthorMetadataProperty.Instance));
    }
}
