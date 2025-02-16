﻿using System.Threading.Tasks;
using MetadataAPI.Data;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties;

public class AdressMetadataPropertyIT
{

    [Theory]
    [InlineData("TestImage_metadata.jpg")]
    public void Read(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        var address = TestUtil.ReadMetadataProperty(filePath, AddressMetadataProperty.Instance);

        Assert.NotNull(address);
        Assert.Equal("Liberty Island 1", address.Sublocation);
        Assert.Equal("10004 New York", address.City);
        Assert.Equal("New York", address.ProvinceState);
        Assert.Equal("Vereinigte Staaten von Amerika", address.Country);
    }

    [Theory]
    [InlineData("TestImage.jpg")]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Write(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, AddressMetadataProperty.Instance, new AddressTag()
        {
            Sublocation = "Constitution Hill",
            City = "SW1A 1AA London",
            ProvinceState = "England",
            Country = "Vereinigtes Königreich",
        });

        var address = TestUtil.ReadMetadataProperty(filePath, AddressMetadataProperty.Instance);

        Assert.NotNull(address);
        Assert.Equal("Constitution Hill", address.Sublocation);
        Assert.Equal("SW1A 1AA London", address.City);
        Assert.Equal("England", address.ProvinceState);
        Assert.Equal("Vereinigtes Königreich", address.Country);
    }

    [Theory]
    [InlineData("TestImage_metadata.jpg")]
    public async Task Remove(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, AddressMetadataProperty.Instance, null);

        var address = TestUtil.ReadMetadataProperty(filePath, AddressMetadataProperty.Instance);

        Assert.Null(address);
    }

    [Theory]
    [InlineData("TestImage_metadata.jpg")]
    public async Task RemovePartialAddress(string fileName)
    {
        string filePath = TestDataProvider.GetFile(fileName);

        await TestUtil.WriteMetadataPropertyAync(filePath, AddressMetadataProperty.Instance, new AddressTag()
        {
            Sublocation = null!,
            City = null!,
            ProvinceState = null!,
            Country = "Deutschland",
        });

        await TestUtil.WriteMetadataPropertyAync(filePath, AddressMetadataProperty.Instance, null);

        var address = TestUtil.ReadMetadataProperty(filePath, AddressMetadataProperty.Instance);

        Assert.Null(address);
    }
}
