using System;
using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class DateTakenMetadataPropertyIT
{
    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    [InlineData(TestConstants.HeicWithMetadata)]
    public void Test_Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var dateTaken = TestUtil.ReadMetadataProperty(filePath, DateTakenMetadataProperty.Instance);

        Assert.Equal(DateTimeKind.Local, dateTaken.Value.Kind);
        Assert.Equal(new DateTime(2020, 4, 18, 16, 46, 51), dateTaken);
    }

    [Theory]
    [InlineData(TestConstants.JpegWithoutMetadata)]
    [InlineData(TestConstants.JpegWithMetadata)]
    //[InlineData(TestConstants.HeicWithoutMetadata)]
    public async Task Test_Write(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var dateTaken = new DateTime(1987, 12, 17, 8, 32, 7);

        await TestUtil.WriteMetadataPropertyAync(filePath, DateTakenMetadataProperty.Instance, dateTaken);

        Assert.Equal(dateTaken, TestUtil.ReadMetadataProperty(filePath, DateTakenMetadataProperty.Instance));
    }


    [Theory]
    [InlineData(TestConstants.JpegWithMetadata)]
    public async Task Test_Remove(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, DateTakenMetadataProperty.Instance, null);

        Assert.Null(TestUtil.ReadMetadataProperty(filePath, DateTakenMetadataProperty.Instance));
    }
}
