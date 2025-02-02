using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WIC;

namespace MetadataAPI;

public class MetadataEncoder : IMetadataWriter
{
    /// <summary>
    /// Padding amount in bit, added on re-encode. 2048 bit by default.
    /// </summary>
    public uint PaddingAmount { get; set; } = 2048;

    /// <summary>
    /// Get or sets whether large thumbnails (greater than 256x256 pixel) are automatically
    /// resized on re-encode if there is not enough space to add new metadata. Disabled by default.
    /// </summary>
    public bool AutoResizeLargeThumbnails { get; set; } = false;

    public IWICBitmapCodecInfo CodecInfo { get; }

    private readonly IWICImagingFactory wic = WICImagingFactory.Create();

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

        decoder = wic.CreateDecoderFromStream(stream, WICDecodeOptions.WICDecodeMetadataCacheOnDemand);

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

        if (metadata.Count == 0)
        {
            return;
        }

        if (!TryEncodeInPlace())
        {
            stream.Seek(0, SeekOrigin.Begin);
            try
            {
                await ReEncodeAsync().ConfigureAwait(false);
            }
            catch (Exception ex) when (AutoResizeLargeThumbnails && ex.HResult == WinCodecError.TOO_MUCH_METADATA)
            {
                await ReEncodeAsync(maxThumbnailSize: 256).ConfigureAwait(false);
            }
        }
    }

    private bool TryEncodeInPlace()
    {
        try
        {
            var frame = decoder.GetFrame(0);

            var inPlaceEncoder = wic.CreateFastMetadataEncoderFromFrameDecode(frame);

            var metadataWriter = new MetadataWriter(inPlaceEncoder.GetMetadataQueryWriter(), CodecInfo);

            foreach (var (name, value) in metadata)
            {
                metadataWriter.SetMetadata(name, value);
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

    private async Task ReEncodeAsync(int maxThumbnailSize = -1)
    {
        try
        {
            using var memoryStream = new MemoryStream();

            var encoder = wic.CreateEncoder(decoder.GetContainerFormat());

            encoder.Initialize(memoryStream, WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache);

            var frame = decoder.GetFrame(0);

            var newFrame = encoder.CreateNewFrame();
            newFrame.Initialize(null);
            newFrame.SetSize(frame.GetSize()); // lossless decoding/encoding
            newFrame.SetResolution(frame.GetResolution()); // lossless decoding/encoding
            newFrame.SetPixelFormat(frame.GetPixelFormat()); // lossless decoding/encoding
            newFrame.SetColorContexts(frame.GetColorContexts());

            try
            {
                var thumbnail = frame.GetThumbnail();
                var thumbnailSize = thumbnail.GetSize();

                if (maxThumbnailSize > 0 && (thumbnailSize.Width > maxThumbnailSize || thumbnailSize.Height > maxThumbnailSize))
                {
                    var scaledThumbnailSize = ScaleSize(thumbnailSize, maxThumbnailSize);
                    var scaledThumbnail = wic.CreateBitmapScaler();
                    scaledThumbnail.Initialize(thumbnail, scaledThumbnailSize, WICBitmapInterpolationMode.WICBitmapInterpolationModeFant);
                    newFrame.SetThumbnail(scaledThumbnail);
                }
                else
                {
                    newFrame.SetThumbnail(thumbnail);
                }
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

            var metadataWriter = new MetadataWriter(newFrame.GetMetadataQueryWriter(), CodecInfo);

            foreach (var (name, value) in metadata)
            {
                metadataWriter.SetMetadata(name, value);
            }

            // Add padding to allow future in-place write operations
            AddPadding(metadataWriter, encoder.GetContainerFormat());

            newFrame.WriteSource(frame);

            newFrame.Commit();
            encoder.Commit();

            await memoryStream.FlushAsync().ConfigureAwait(false);
            memoryStream.Position = 0;
            stream.Position = 0;
            stream.SetLength(0);
            await memoryStream.CopyToAsync(stream).ConfigureAwait(false);
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

    private void AddPadding(MetadataWriter metadataWriter, Guid containerFormat)
    {
        if (PaddingAmount == 0)
        {
            return;
        }
        if (containerFormat == ContainerFormat.Jpeg)
        {
            metadataWriter.SetMetadata("/app1/ifd/PaddingSchema:Padding", PaddingAmount);
            metadataWriter.SetMetadata("/app1/ifd/exif/PaddingSchema:Padding", PaddingAmount);
            metadataWriter.SetMetadata("/xmp/PaddingSchema:Padding", PaddingAmount);
        }
        else if (containerFormat == ContainerFormat.Tiff)
        {
            metadataWriter.SetMetadata("/ifd/PaddingSchema:padding", PaddingAmount);
            metadataWriter.SetMetadata("/ifd/exif/PaddingSchema:padding", PaddingAmount);
        }
    }

    private WICSize ScaleSize(WICSize size, int max)
    {
        if (size.Width > size.Height)
        {
            return new WICSize(max, (int)Math.Round(size.Height * ((float)max / size.Width)));
        }
        else
        {
            return new WICSize((int)Math.Round(size.Width * ((float)max / size.Height)), max);
        }
    }
}
