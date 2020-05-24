using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI
{
    internal static class FileExtensions
    {
        public static IReadOnlyCollection<string> Jpeg = new HashSet<string> { ".jpe", ".jpeg", ".jpg" };
        public static IReadOnlyCollection<string> Tiff = new HashSet<string> { ".tiff", ".tif" };
        public static IReadOnlyCollection<string> Heif = new HashSet<string> { ".heic", ".heif" };
        public static IReadOnlyCollection<string> Png = new HashSet<string> { ".png" };
    }
}
