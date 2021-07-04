﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI.Data;
using MetadataAPI.Properties;
using Xunit;

namespace MetadataAPITest.IntegrationTest.Properties
{
    public class PeopleMetadataPropertyIT
    {
        [Theory]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Read(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            var people = await TestUtil.ReadMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance);

            Assert.Equal(1, people.Count);
            Assert.Equal("Test", people[0].Name);
        }

        [Theory]
        [InlineData(TestConstants.JpegWithoutMetadata)]
        [InlineData(TestConstants.JpegWithMetadata)]
        [InlineData(TestConstants.JpegWithEmptyPeopleTags)]
        public async Task Test_Write(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            var people = new[] { new PeopleTag("New Test"), new PeopleTag("New Test 02"), new PeopleTag("New Test 03") };

            await TestUtil.WriteMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance, people);

            Assert.Equal(people, await TestUtil.ReadMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance));
        }

        [Theory]
        [InlineData(TestConstants.JpegWithoutMetadata)]
        public async Task Test_Write_LessEntriesThanBefore(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);
            var people = new[] { new PeopleTag("Test 01"), new PeopleTag("Test 02"), new PeopleTag("Test 03") };
            await TestUtil.WriteMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance, people);

            people = new[] { new PeopleTag("Test 02"), new PeopleTag("Test 03") };
            await TestUtil.WriteMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance, people);
            Assert.Equal(people, await TestUtil.ReadMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance));

            people = new[] { new PeopleTag("Test 03") };
            await TestUtil.WriteMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance, people);
            Assert.Equal(people, await TestUtil.ReadMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance));
        }

        [Theory]
        [InlineData(TestConstants.JpegWithMetadata)]
        public async Task Test_Remove(string fileName)
        {
            string filePath = TestDataProvider.GetFile(fileName);

            await TestUtil.WriteMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance, Array.Empty<PeopleTag>());

            Assert.Empty(await TestUtil.ReadMetadataPropertyAync(filePath, PeopleMetadataProperty.Instance));
        }
    }
}
