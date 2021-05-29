using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MetadataAPI;
using WIC;

namespace MetadataAPITest.UnitTest
{
    public class MetadataPropertyExtensionTest
    {
        private class TestMetadataProperty : IReadonlyMetadataProperty
        {
            public string Identifier => throw new NotImplementedException();

            public IReadOnlyCollection<Guid> SupportedFormats { get; } = new HashSet<Guid>() { ContainerFormat.Jpeg };

            public object Read(IMetadataReader metadataReader)
            {
                throw new NotImplementedException();
            }
        }

        [Theory]
        [InlineData(".jpg")]
        [InlineData(".jpe")]
        [InlineData(".jpeg")]
        public void Test_IsSupported_True(string fileExtension) 
        {
            var testMetadataProperty = new TestMetadataProperty();

            Assert.True(testMetadataProperty.IsSupported(fileExtension));
        }

        [Theory]
        [InlineData(".png")]
        [InlineData(".gif")]
        [InlineData(".txt")]
        public void Test_IsSupported_False(string fileExtension)
        {
            var testMetadataProperty = new TestMetadataProperty();
                        
            Assert.False(testMetadataProperty.IsSupported(fileExtension));
        }

    }
}
