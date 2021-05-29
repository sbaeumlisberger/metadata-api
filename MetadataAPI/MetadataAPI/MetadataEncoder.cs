using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WIC;

namespace MetadataAPI
{
    public class MetadataEncoder : IMetadataWriter
    {
        /// <summary>
        /// Padding amount in bit, added on re-encode. 2048 bit by default.
        /// </summary>
        public uint PaddingAmount { get; set; } = 2048;

        public IWICBitmapCodecInfo CodecInfo { get; }

        private readonly WICImagingFactory wic = new WICImagingFactory();

        private readonly Stream stream;

        private readonly IWICBitmapDecoder decoder;

        private readonly MetadataReader metadataReader;

        private readonly List<(string, object?)> metadata = new List<(string, object?)>();
        
        [Obsolete]
        public MetadataEncoder(Stream stream, string fileType) : this(stream)
        {
        }

        public MetadataEncoder(Stream stream)
        {
            if (stream.CanWrite is false) 
            {
                throw new ArgumentException("Stream must be writable", nameof(stream));
            }

            this.stream = stream;

            decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);

            CodecInfo = decoder.GetDecoderInfo();

            metadataReader = new MetadataReader(decoder.GetFrame(0).GetMetadataQueryReader(), CodecInfo);
        }

        public object? GetMetadata(string key)
        {
            return metadataReader.GetMetadata(key);
        }

        public IEnumerable<string> GetKeys()
        {
            return metadataReader.GetKeys();
        }

        public IMetadataReader? GetMetadataBlock(string key)
        {
            return metadataReader.GetMetadataBlock(key);
        }

        public void SetMetadata(string name, object? value)
        {
            metadata.Add((name, value));
        }

        public async Task EncodeAsync()
        {
            if (stream is null)
            {
                throw new InvalidOperationException();
            }

            if (!metadata.Any())
            {
                return;
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
                var frame = decoder.GetFrame(0);

                var inPlaceEncoder = wic.CreateFastMetadataEncoderFromFrameDecode(frame);

                var metadataQueryWriter = inPlaceEncoder.GetMetadataQueryWriter();

                foreach (var (name, value) in metadata)
                {
                    if (value is null)
                    {
                        metadataQueryWriter.RemoveMetadataByName(name);
                    }
                    else
                    {
                        metadataQueryWriter.SetMetadataByName(name, value);
                    }
                }

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
            catch (Exception ex) when (ex.HResult == WinCodecError.TOO_MUCH_METADATA
                || ex.HResult == WinCodecError.INSUFFICIENT_BUFFER
                || ex.HResult == WinCodecError.IMAGE_METADATA_HEADER_UNKNOWN)
            {
                return false;
            }
        }

        private async Task ReEncodeAsync()
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var encoder = wic.CreateEncoder(decoder.GetContainerFormat());

                    encoder.Initialize(memoryStream.AsCOMStream(), WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache);

                    var frame = decoder.GetFrame(0);

                    var newFrame = encoder.CreateNewFrame();
                    newFrame.Initialize(null);
                    newFrame.SetSize(frame.GetSize()); // lossless decoding/encoding
                    newFrame.SetResolution(frame.GetResolution()); // lossless decoding/encoding
                    newFrame.SetPixelFormat(frame.GetPixelFormat()); // lossless decoding/encoding
                    newFrame.SetColorContexts(frame.GetColorContexts());
                    try
                    {
                        newFrame.SetThumbnail(frame.GetThumbnail());
                    }
                    catch (Exception exception) when (exception.HResult == WinCodecError.CODEC_NO_THUMBNAIL)
                    {
                        // no thumbnail available
                    }

                    var metadataBlockWriter = newFrame.AsMetadataBlockWriter();

                    if (metadataBlockWriter is null)
                    {
                        throw new NotSupportedException("The file format does not support any metadata.");
                    }

                    metadataBlockWriter.InitializeFromBlockReader(frame.AsMetadataBlockReader());

                    var metadataQueryWriter = newFrame.GetMetadataQueryWriter();

                    foreach (var (name, value) in metadata)
                    {
                        if (value is null)
                        {
                            metadataQueryWriter.RemoveMetadataByName(name);
                        }
                        else
                        {
                            metadataQueryWriter.SetMetadataByName(name, value);
                        }
                    }

                    // Add padding to allow future in-place write operations
                    AddPadding(metadataQueryWriter, encoder.GetContainerFormat());

                    newFrame.WriteSource(frame);

                    newFrame.Commit();
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

        private void AddPadding(IWICMetadataQueryWriter metadataWriter, Guid containerFormat)
        {
            if (PaddingAmount == 0)
            {
                return;
            }
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
