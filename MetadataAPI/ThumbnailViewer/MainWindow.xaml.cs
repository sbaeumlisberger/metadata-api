using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThumbnailViewer
{

    public partial class MainWindow : Window
    {
        private static readonly HashSet<string> MetadataSupportingFileTypes = new HashSet<string>() { ".jpg", ".jpeg", ".jpe", ".tif", ".tiff" };


        public MainWindow()
        {
            InitializeComponent();

            string filePath = Environment.GetCommandLineArgs()[1];

            try
            {
                using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                    image.Source = GetOrientedBitmapSource(filePath, decoder, decoder.Frames[0].Thumbnail);
                }
            }
            catch
            {
                Debug.WriteLine("Could not get thumbnail for " + filePath);
            }
        }


        public static BitmapSource GetOrientedBitmapSource(string path, BitmapDecoder decoder, BitmapSource imageSource)
        {
            if (MetadataSupportingFileTypes.Contains(Path.GetExtension(path).ToLower()))
            {
                ushort? orientation = ((BitmapMetadata)decoder.Frames[0].Metadata).GetQuery("System.Photo.Orientation") as ushort?;
                switch (orientation)
                {
                    case 6: return new TransformedBitmap(imageSource, new RotateTransform(90));
                    case 3: return new TransformedBitmap(imageSource, new RotateTransform(180));
                    case 8: return new TransformedBitmap(imageSource, new RotateTransform(270));
                }
            }
            return imageSource;
        }




    }
}
