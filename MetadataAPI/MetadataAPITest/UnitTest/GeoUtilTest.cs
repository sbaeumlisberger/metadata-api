using System;
using System.Collections.Generic;
using System.Text;
using DotNetToolkit.Foundation;
using MetadataAPI.Helper;
using Xunit;

namespace MetadataAPITest.UnitTest
{
    public class GeoUtilTest
    {

        [Theory]
        [InlineData(40.6901601, 40, 41, 24.576)]
        [InlineData(-137.8563170, 137, 51, 22.741)]
        public void Test_DecimalToDegreesMinutesSeconds(double decimalValue, uint expectedDegrees, uint expectedMinutes, double expectedSeconds)
        {
            var result = GeoUtil.DecimalToDegreesMinutesSeconds(decimalValue);
            Assert.Equal(expectedDegrees, result.Degrees);
            Assert.Equal(expectedMinutes, result.Minutes);
            Assert.Equal(expectedSeconds, result.Seconds, 3);
        }

        [Theory]
        [InlineData(40, 41, 24.576, 40.6901601)]
        [InlineData(137, 51, 22.741, 137.8563170)]
        public void Test_DegreesMinutesSecondsToDecimal(uint degrees, uint minutes, double seconds, double expectedDecimalValue)
        {
            var result = GeoUtil.DegreesMinutesSecondsToDecimal(degrees, minutes, seconds);
            Assert.Equal(expectedDecimalValue, result, 6);
        }

    }
}
