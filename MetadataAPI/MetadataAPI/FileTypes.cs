using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI
{
    internal static class FileTypes
    {
        public static IReadOnlyCollection<string> JpegExtensions = new HashSet<string> { ".jpe", ".jpeg", ".jpg" };
        public static IReadOnlyCollection<string> TiffExtensions = new HashSet<string> { ".tiff", ".tif" };
        public static IReadOnlyCollection<string> HeifExtensions = new HashSet<string> { ".heic", ".heif" };
        public static IReadOnlyCollection<string> PngExtensions = new HashSet<string> { ".png" };
    }
}
