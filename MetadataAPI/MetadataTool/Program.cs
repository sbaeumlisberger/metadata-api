using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using MetadataAPI;
using WIC;

namespace MetadataTool
{
    class Program
    {
        private static readonly IDictionary<string, string> ReadableNames = new Dictionary<string, string>()
        {
            {"/app0/{ushort=0}","Exif Version" },
            {"/app0/{ushort=1}","Units" },
            {"/app0/{ushort=2}", "DpiX" },
            {"/app0/{ushort=3}", "DpiY" },
            {"/app0/{ushort=4}", "Xthumbnail" },
            {"/app0/{ushort=5}", "Ythumbnail" },
            {"/app0/{ushort=6}", "ThumbnailData" },
            {"/app1/thumb", "Thumbnail IFD" },
            {"/app1/{ushort=1}/{}", "Thumbnail IFD" },
            {"/app1/{ushort=0}/{ushort=256}", "ImageWidth" },
            {"/app1/{ushort=0}/{ushort=257}", "ImageLength" },
            {"/app1/{ushort=0}/{ushort=258}", "BitsPerSample" },
            {"/app1/{ushort=0}/{ushort=259}", "Compression" },
            {"/app1/{ushort=0}/{ushort=262}", "PhotometricInterpretation" },
            {"/app1/{ushort=0}/{ushort=274}", "Orientation" },
            {"/app1/{ushort=0}/{ushort=277}", "SamplesPerPixel" },
            {"/app1/{ushort=0}/{ushort=284}", "PlanarConfiguration" },
            {"/app1/{ushort=0}/{ushort=530}", "YCbCrSubSampling" },
            {"/app1/{ushort=0}/{ushort=531}", "YCbCrPositioning" },
            {"/app1/{ushort=0}/{ushort=282}", "XResolution" },
            {"/app1/{ushort=0}/{ushort=283}", "YResolution" },
            {"/app1/{ushort=0}/{ushort=296}", "ResolutionUnit" },
            {"/app1/{ushort=0}/{ushort=306}", "DateTime"   },
            {"/app1/{ushort=0}/{ushort=270}", "ImageDescription" },
            {"/app1/{ushort=0}/{ushort=271}", "Make" },
            {"/app1/{ushort=0}/{ushort=272}", "Model" },
            {"/app1/{ushort=0}/{ushort=305}", "Software" },
            {"/app1/{ushort=0}/{ushort=315}", "Artist" },
            {"/app1/{ushort=0}/{ushort=33432}", "Copyright" },
            {"/app1/{ushort=0}/{ushort=338}", "ExtraSamples" },
            {"/app1/{ushort=0}/{ushort=254}", "NewSubfileType" },
            {"/app1/{ushort=0}/{ushort=278}", "RowsPerStrip" },
            {"/app1/{ushort=0}/{ushort=279} ", "StripByteCounts" },
            {"/app1/{ushort=0}/{ushort=273}", "StripOffsets" }
        };

        static void Main(string[] args)
        {
            string filePath = args[1];

            string command = args[0];

            if (command == "show")
            {
                ShowMetadata(filePath);
            }
            else if (command == "remove")
            {
                RemoveMetadata(filePath);
                Console.WriteLine("metadata successfully removed");
            }
            else if (command == "padding")
            {
                uint padding = uint.Parse(GetArg(args, 2, "2048"));
                AddPadding(filePath, padding);
                Console.WriteLine("padding successfully added");
            }
        }

        private static string GetArg(string[] args, int index, string fallbackValue)
        {
            return args.Length > index ? args[index] : fallbackValue;
        }

        private static void ShowMetadata(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataDecoder = new MetadataDecoder(stream);

                foreach (var entry in ReadMetadata(metadataDecoder))
                {
                    var value = entry.Value;

                    if (value is WICBlob blob)
                    {
                        value = ArrayToString(blob.Bytes);
                    }
                    if (value is Array array)
                    {
                        value = ArrayToString(array);
                    }

                    if (ReadableNames.TryGetValue(entry.Key, out string readableName))
                    {
                        Console.WriteLine($"{entry.Key} ({readableName}): {value}");
                    }
                    else
                    {
                        Console.WriteLine($"{entry.Key}: {value}");
                    }
                }
            }
        }

        private static string ArrayToString(Array array)
        {
            var elements = array.Length <= 10 ? array.Cast<object>().ToArray() : array.Cast<object>().Take(10).Append("...").ToArray();
            return array.GetType().GetElementType().Name + "[" + array.Length + "] {" + string.Join(", ", elements) + "}";

        }

        private static void RemoveMetadata(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                WICImagingFactory wic = new WICImagingFactory();

                var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);

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

                    newFrame.WriteSource(frame);

                    newFrame.Commit();
                    encoder.Commit();

                    memoryStream.Flush();
                    memoryStream.Position = 0;
                    stream.Position = 0;
                    stream.SetLength(0);
                    memoryStream.CopyTo(stream);
                }
            }
        }

        private static void AddPadding(string filePath, uint paddingAmount)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                WICImagingFactory wic = new WICImagingFactory();

                var decoder = wic.CreateDecoderFromStream(stream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);

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
                    catch (COMException exception) when (exception.HResult == WinCodecError.CODEC_NO_THUMBNAIL)
                    {

                    }

                    var metadataBlockWriter = newFrame.AsMetadataBlockWriter();

                    if (metadataBlockWriter is null)
                    {
                        throw new NotSupportedException("The file format does not support any metadata.");
                    }

                    metadataBlockWriter.InitializeFromBlockReader(frame.AsMetadataBlockReader());

                    var metadataQueryWriter = newFrame.GetMetadataQueryWriter();

                    if (encoder.GetContainerFormat() == ContainerFormat.Jpeg)
                    {
                        metadataQueryWriter.SetMetadataByName("/app1/ifd/PaddingSchema:Padding", paddingAmount);
                        metadataQueryWriter.SetMetadataByName("/app1/ifd/exif/PaddingSchema:Padding", paddingAmount);
                        metadataQueryWriter.SetMetadataByName("/xmp/PaddingSchema:Padding", paddingAmount);
                    }
                    else if (encoder.GetContainerFormat() == ContainerFormat.Tiff)
                    {
                        metadataQueryWriter.SetMetadataByName("/ifd/PaddingSchema:padding", paddingAmount);
                        metadataQueryWriter.SetMetadataByName("/ifd/exif/PaddingSchema:padding", paddingAmount);
                    }

                    newFrame.WriteSource(frame);

                    newFrame.Commit();
                    encoder.Commit();

                    memoryStream.Flush();
                    memoryStream.Position = 0;
                    stream.Position = 0;
                    stream.SetLength(0);
                    memoryStream.CopyTo(stream);
                }
            }
        }

        private static void RemoveMetadataRecursive(IWICMetadataQueryWriter metadataQueryWriter)
        {
            foreach (string name in metadataQueryWriter.GetNames())
            {
                if (metadataQueryWriter.GetMetadataByName(name) is IWICMetadataQueryWriter _metadataQueryWriter)
                {
                    RemoveMetadataRecursive(_metadataQueryWriter);
                }
                else
                {
                    try
                    {
                        metadataQueryWriter.RemoveMetadataByName(name);
                    }
                    catch { }
                }
            }
        }

        private static IDictionary<string, object> ReadMetadata(IMetadataReader metadataReader)
        {
            IDictionary<string, object> metadata = new Dictionary<string, object>();

            IDictionary<string, object> ReadMetadataRecursive(IMetadataReader metadataReader, string path)
            {
                foreach (string key in metadataReader.GetKeys())
                {
                    try
                    {
                        if (metadataReader.GetMetadataBlock(key) is IMetadataReader _metadataReader)
                        {
                            ReadMetadataRecursive(_metadataReader, path + key);
                        }
                        else
                        {
                            metadata.Add(path + key, metadataReader.GetMetadata(key));
                        }
                    }
                    catch (Exception exception)
                    {
                        metadata.Add(path + key, exception.Message);
                    }
                }
                return metadata;
            }

            return ReadMetadataRecursive(metadataReader, "");
        }
    }

}
