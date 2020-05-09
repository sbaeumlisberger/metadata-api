using MetadataAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WIC;

namespace MetadataAPI
{

    /// <summary>
    /// Helper class for reading and writing bitmap properties.
    /// </summary>
    public static class BitmapPropertiesHelper
    {

        /// <summary>
        /// Padding amount in bit, added on re-encode 
        /// </summary>
        private const uint PaddingAmount = 4096;

        private const int WINCODEC_ERR_PROPERTY_NOT_SUPPORTED = unchecked((int)0x88982F41);
        private const int WINCODEC_ERR_UNSUPPORTED_OPERATION = unchecked((int)0x88982F81);
        private const int WINCODEC_ERR_TOOMUCHMETADATA = unchecked((int)0x88982F52);
        private const int WINCODEC_ERR_INSUFFICIENTBUFFER = unchecked((int)0x88982F8C);
        private const int WINCODEC_ERR_IMAGE_METADATA_HEADER_UNKNOWN = unchecked((int)0x88982F63);

        public static async Task ReadAsync(Stream stream, Action<IWICMetadataQueryReader> readPropertiesCallback)        
        {
            var wic = new WICImagingFactory();

            var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);

            var frame = decoder.GetFrame(0);

            var metadataReader = frame.GetMetadataQueryReader();

            readPropertiesCallback(metadataReader);
        }

        public static async Task WriteAsync(Stream stream, Action<IWICMetadataQueryWriter> writePropertiesCallback)
        {
            if (!stream.CanRead)             
            {
                throw new ArgumentException("Stream must be readable", nameof(stream));
            }
            if(!stream.CanWrite) 
            {
                throw new ArgumentException("Stream must be writable", nameof(stream));
            }

            if (!WriteInPlace(stream, writePropertiesCallback))
            {
                stream.Position = 0;
                await WriteReEncodeAsync(stream, writePropertiesCallback);
            }
        }

        private static bool WriteInPlace(Stream stream, Action<IWICMetadataQueryWriter> writePropertiesCallback)
        {
            try
            {
                var wic = new WICImagingFactory();

                var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);

                var frame = decoder.GetFrame(0);

                var encoder = wic.CreateFastMetadataEncoderFromFrameDecode(frame);

                var metadataWriter = encoder.GetMetadataQueryWriter();

                writePropertiesCallback(metadataWriter);

                encoder.Commit();

                return true;
            }
            catch (Exception ex) when (ex.HResult == WINCODEC_ERR_PROPERTY_NOT_SUPPORTED)
            {
                throw new NotSupportedException("The file format does not support the requested metadata.", ex);
            }
            catch (Exception ex) when (ex.HResult == WINCODEC_ERR_UNSUPPORTED_OPERATION)
            {
                return false;
            }
            catch (Exception ex) when (ex.HResult == WINCODEC_ERR_TOOMUCHMETADATA
                || ex.HResult == WINCODEC_ERR_INSUFFICIENTBUFFER
                || ex.HResult == WINCODEC_ERR_IMAGE_METADATA_HEADER_UNKNOWN)
            {
                return false;
            }
        }

        private static async Task WriteReEncodeAsync(Stream stream, Action<IWICMetadataQueryWriter> writePropertiesCallback)
        {
            try
            {
                var wic = new WICImagingFactory();

                var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand/*lossless decoding/encoding*/);

                var frame = decoder.GetFrame(0);

                using (var memoryStream = new MemoryStream())
                {
                    var encoder = wic.CreateEncoder(decoder.GetContainerFormat());

                    encoder.Initialize(memoryStream.AsCOMStream(), WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache);

                    var encoderFrame = encoder.CreateNewFrame();
                    encoderFrame.Initialize(null);
                    encoderFrame.SetSize(frame.GetSize()); // lossless decoding/encoding
                    encoderFrame.SetResolution(frame.GetResolution()); // lossless decoding/encoding
                    encoderFrame.SetPixelFormat(frame.GetPixelFormat()); // lossless decoding/encoding

                    var metadataBlockWriter = encoderFrame.AsMetadataBlockWriter();

                    if (metadataBlockWriter is null)
                    {
                        throw new NotSupportedException("The file format does not support any metadata.");
                    }

                    metadataBlockWriter.InitializeFromBlockReader(frame.AsMetadataBlockReader());

                    var metadataWriter = encoderFrame.GetMetadataQueryWriter();

                    writePropertiesCallback(metadataWriter);

                    // Add padding to allow future in-place write operations
                    AddPadding(metadataWriter, encoder.GetContainerFormat()); 

                    encoderFrame.WriteSource(frame);

                    encoderFrame.Commit();
                    encoder.Commit();

                    await memoryStream.FlushAsync().ConfigureAwait(false);
                    memoryStream.Position = 0;
                    stream.Position = 0;
                    stream.SetLength(0);
                    await memoryStream.CopyToAsync(stream).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex.HResult == WINCODEC_ERR_PROPERTY_NOT_SUPPORTED)
            {
                throw new NotSupportedException("The file format does not support the requested metadata.", ex);
            }
            catch (Exception ex) when (ex.HResult == WINCODEC_ERR_UNSUPPORTED_OPERATION)
            {
                throw new NotSupportedException("The file format does not support any metadata.", ex);
            }
        }

        private static void AddPadding(IWICMetadataQueryWriter metadataWriter, Guid containerFormat) 
        {
            if (containerFormat == ContainerFormat.Jpeg)
            {
                metadataWriter.SetMetadataByName("/app1/ifd/PaddingSchema:Padding", PaddingAmount);
                metadataWriter.SetMetadataByName("/app1/ifd/exif/PaddingSchema:Padding", PaddingAmount);
                metadataWriter.SetMetadataByName("/xmp/PaddingSchema:Padding", PaddingAmount);
            }
            else if (containerFormat == ContainerFormat.Tiff)
            {
                metadataWriter.SetMetadataByName("/ifd/PaddingSchema:padding", PaddingAmount);
                metadataWriter.SetMetadataByName("/ifd/exif/PaddingSchema:padding", PaddingAmount);
            }
        }

    }

}
