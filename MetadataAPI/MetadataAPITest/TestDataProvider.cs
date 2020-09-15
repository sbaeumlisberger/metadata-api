using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MetadataAPITest
{
    public static class TestDataProvider
    {
        private const string TestWorkspaceDirectoryName = "TestWorkspace";

        private static readonly string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

        private static readonly string WorkspacePath = Path.Combine(BaseDirectoryPath, TestWorkspaceDirectoryName);

        public static string GetFile(string relativeFilePath, [CallerFilePath] string callerFile = null, [CallerMemberName] string callerMember = null)
        {
            string filePath = Path.Combine(BaseDirectoryPath, "TestData", relativeFilePath);

            string testDirectoryPath = Path.Combine(WorkspacePath, Path.GetFileNameWithoutExtension(callerFile), callerMember);

            CreateEmptyDirectory(testDirectoryPath);

            string testFilePath = Path.Combine(testDirectoryPath, relativeFilePath);

            File.Copy(filePath, testFilePath);

            return testFilePath;
        }

        private static void CreateEmptyDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }

    }
}
