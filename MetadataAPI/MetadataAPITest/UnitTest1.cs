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
        public async Task Test_ReEncode()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "/TestBitmap.jpg";

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var metadataWriter = new MetadataWriter(stream, Path.GetExtension(filePath));
                metadataWriter.SetMetadata(TitleMetadataProperty.Instance, "dsggggggggggggggg" +
                    "sdggggggggggggggggggg" +
                    " 444254dsgfdghgfjjgggggggggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg" + "sdggggggggggggggggggg" +
                    " 444254dsgfdghgfjjgggggggggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg" + "sdggggggggggggggggggg" +
                    " 444254dsgfdghgfjjgggggggggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg" + " 444254dsgfdghgfjjgggggggggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg" + "sdggggggggggggggggggg" +
                    " 444254dsgfdghgfjjgggggggggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg" + "sdggggggggggggggggggg" +
                    " 444254dsgfdghgfjjgggggggggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg" + " 444254dsgfdghgfjjgggggggggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg" + "sdggggggggggggggggggg" +
                    " 444254dsgfdghgfjjgggggggsdfeeggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg" + "sdggggggggggggggggggg" +
                    " 444254dsgfdghgfjjgggggggggggghgghgfdsdfffffffffffffff442442" +
                    "sadetr  retttttttttttttttttttttttdfgfd42g2d4g5d45g " +
                    "dfgfdgffg " +
                    "dfgdggfdgdg");
                await metadataWriter.CommitAsync();
            }
        }
    }
}
