using System;
using System.Collections.Generic;
using System.Text;
using DotNetToolkit.Foundation;

namespace MetadataAPI.Helper
{
    public static class GeoUtil
    {

        public static (uint Degrees, uint Minutes, double Seconds) DecimalToDegreesMinutesSeconds(double decimalValue)
        {     
            double absoluteDecimalValue = Math.Abs(decimalValue);
            uint degrees = (uint)absoluteDecimalValue;
            uint minutes = (uint)((absoluteDecimalValue - degrees) * 60);
            double seconds = (absoluteDecimalValue - degrees - minutes / 60d) * 3600;
            return (degrees, minutes, seconds);
        }

        public static double DegreesMinutesSecondsToDecimal(uint degrees, uint minutes, double seconds)
        {
            return ((seconds / 60d) + minutes) / 60d + degrees;
        }
    }
}
