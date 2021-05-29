using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIC;

namespace MetadataAPI
{
    public static class MetadataPropertyExtension
    {

        private static Dictionary<Guid, IWICBitmapCodecInfo>? codecsByFormat = null;

        private static Dictionary<Guid, IWICBitmapCodecInfo> LoadCodecs()
        {
            var codecInfoByFormat = new Dictionary<Guid, IWICBitmapCodecInfo>();
            var wic = new WICImagingFactory();
            foreach (var decoderInfo in wic
                .CreateComponentEnumerator(WICComponentType.WICDecoder, WICComponentEnumerateOptions.WICComponentEnumerateDefault)
                .AsEnumerable()
                .OfType<IWICBitmapDecoderInfo>())
            {
                codecInfoByFormat.Add(decoderInfo.GetContainerFormat(), decoderInfo);
            }
            return codecInfoByFormat;
        }

        public static bool IsSupported(this IReadonlyMetadataProperty metadataProperty, string fileExtension)
        {
            fileExtension = fileExtension.ToLower();
            codecsByFormat ??= LoadCodecs();
            return metadataProperty.SupportedFormats.Any(format =>
            {
                return codecsByFormat[format].GetFileExtensions().Contains(fileExtension);
            });
        }
    }
}
