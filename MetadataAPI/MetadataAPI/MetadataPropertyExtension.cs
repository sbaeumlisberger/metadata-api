using System;
using System.Collections.Generic;
using System.Linq;
using WIC;

namespace MetadataAPI;

public static class MetadataPropertyExtension
{

    private static Dictionary<Guid, string[]>? fileExtensionsByFormat = null;

    private static string[] GetFileExtensionsByFormat(Guid format)
    {
        if (fileExtensionsByFormat is null)
        {
            fileExtensionsByFormat = new Dictionary<Guid, string[]>();
            var wic = WICImagingFactory.Create();
            foreach (var decoderInfo in wic
                .CreateComponentEnumerator(WICComponentType.WICDecoder, WICComponentEnumerateOptions.WICComponentEnumerateDefault)
                .AsEnumerable()
                .OfType<IWICBitmapDecoderInfo>())
            {
                fileExtensionsByFormat.Add(decoderInfo.GetContainerFormat(), decoderInfo.GetFileExtensions());
            }
        }
        return fileExtensionsByFormat[format];
    }

    public static bool IsSupported(this IReadonlyMetadataProperty metadataProperty, string fileExtension)
    {
        fileExtension = fileExtension.ToLower();
        return metadataProperty.SupportedFormats.Any(format =>
        {
            return GetFileExtensionsByFormat(format).Contains(fileExtension);
        });
    }
}
