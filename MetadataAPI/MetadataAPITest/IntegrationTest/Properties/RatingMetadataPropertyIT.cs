﻿using System.Threading.Tasks;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class RatingMetadataPropertyIT
{

    [Theory]
    [InlineData("TestImage_metadata.jpg")]
    public void Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        Assert.Equal(3, TestUtil.ReadMetadataProperty(filePath, RatingMetadataProperty.Instance));
    }

    [Theory]
    [InlineData("TestImage.jpg")]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Write(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, RatingMetadataProperty.Instance, 4);

        Assert.Equal(4, TestUtil.ReadMetadataProperty(filePath, RatingMetadataProperty.Instance));
    }

}
