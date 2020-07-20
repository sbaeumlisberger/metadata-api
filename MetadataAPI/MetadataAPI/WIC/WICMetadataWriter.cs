using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MetadataAPI.Provider;
using WIC;

namespace MetadataAPI.WIC
{
    public class WICMetadataWriter : IMetadataWriter
    {

        /// <summary>
        /// Padding amount in bit, added on re-encode 
        /// </summary>
        private const uint PaddingAmount = 4096;

        private Stream stream;

        private IWICFastMetadataEncoder inPlaceEncoder;

        private WICWriteMetadata writeMetadata;

        private readonly WICImagingFactory wic = new WICImagingFactory();

        public IWriteMetadata SetStream(Stream stream, string fileType)
        {
            this.stream = stream;

            var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);

            var frame = decoder.GetFrame(0);

            inPlaceEncoder = wic.CreateFastMetadataEncoderFromFrameDecode(frame);

            var metadataQueryWriter = inPlaceEncoder.GetMetadataQueryWriter();

            writeMetadata = new WICWriteMetadata(fileType, metadataQueryWriter);

            return writeMetadata;
        }

        public async Task CommitAsync() 
        {
            if (stream is null)
            {
                throw new InvalidOperationException();
            }

            if (!TryEncodeInPlace()) 
            {
                stream.Seek(0, SeekOrigin.Begin);
                await ReEncodeAsync();
            }
        }

        private bool TryEncodeInPlace() 
        {
            try
            {
                inPlaceEncoder.Commit();
                return true;
            }
            catch (Exception ex) when (ex.HResult == WinCodecError.PROPERTY_NOT_SUPPORTED)
            {
                throw new NotSupportedException("The file format does not support the requested metadata.", ex);
            }
            catch (Exception ex) when (ex.HResult == WinCodecError.UNSUPPORTED_OPERATION)
            {
                return false;
            }
            catch (Exception ex) when (ex.HResult == WinCodecError.TOOMUCHMETADATA
                || ex.HResult == WinCodecError.INSUFFICIENTBUFFER
                || ex.HResult == WinCodecError.IMAGE_METADATA_HEADER_UNKNOWN)
            {
                return false;
            }
        }

        private async Task ReEncodeAsync() 
        {
            try
            {
                var decoder = wic.CreateDecoderFromStream(stream, WICDecodeOptions.WICDecodeMetadataCacheOnDemand/*lossless decoding/encoding*/);

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
                    
                    foreach (var (name, value) in writeMetadata.Requests) 
                    {
                        metadataWriter.SetMetadataByName(name, value);
                    }

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
            catch (Exception ex) when (ex.HResult == WinCodecError.PROPERTY_NOT_SUPPORTED)
            {
                throw new NotSupportedException("The file format does not support the requested metadata.", ex);
            }
            catch (Exception ex) when (ex.HResult == WinCodecError.UNSUPPORTED_OPERATION)
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
