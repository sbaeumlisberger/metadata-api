using System;
using System.IO;
using System.Threading.Tasks;
using MetadataAPI;
using MetadataAPI.Definitions;
using Xunit;
using WIC;
using System.Runtime.InteropServices;

namespace MetadataAPITest
{
    public class UnitTest1
    {

        [Fact]
        public async Task Test1()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "/TestBitmap.jpg";

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataReader = new MetadataReader(stream, Path.GetExtension(filePath));
                var people = metadataReader.GetMetadata(PeopleMetadataProperty.Instance);
                Console.WriteLine(people);
            }

            //var authorProperty = new AuthorMetadataProperty();
            //var dateTakenProperty = new DateTakenMetadataProperty();

            //WICMetadataReader wicReader = new WICMetadataReader();
            //WICMetadataWriter wicWriter = new WICMetadataWriter();

            //string filePath = AppDomain.CurrentDomain.BaseDirectory + "/TestBitmap.heic";

            //using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            //{
            //    wicReader.FileType = Path.GetExtension(filePath);
            //    await BitmapPropertiesHelper.ReadAsync(stream, (queryReader) =>
            //    {
            //        wicReader.SetWICMetadataQueryReader(queryReader);
            //        var author = authorProperty.Read(wicReader);
            //        Console.WriteLine(author);
            //        var dateTaken = dateTakenProperty.Read(wicReader);
            //        Console.WriteLine(dateTaken);
            //    });
            //}

            //using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            //{
            //    wicWriter.FileType = Path.GetExtension(filePath);
            //    await BitmapPropertiesHelper.WriteAsync(stream, (queryWriter) =>
            //    {
            //        wicWriter.SetWICMetadataQueryWriter(queryWriter);

            //        //queryWriter.SetMetadataByName("System.Copyright", "TEST");

            //        //authorProperty.Write(wicWriter, new string[] { "Author1", "Author2" });
            //        dateTakenProperty.Write(wicWriter, DateTime.Now);

            //        Console.WriteLine("Success");
            //    });
            //}
        }
    }
}
