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

        private static readonly string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory!;

        private static readonly string WorkspacePath = Path.Combine(BaseDirectoryPath, TestWorkspaceDirectoryName);

        static TestDataProvider()
        {
            if (Directory.Exists(WorkspacePath))
            {
                Directory.Delete(WorkspacePath, true);
            }
        }

        public static string GetFile(string relativeFilePath, string postfix = "", [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "")
        {
            string filePath = Path.Combine(BaseDirectoryPath, "TestData", relativeFilePath);

            string testDirectoryPath = Path.Combine(WorkspacePath, Path.GetFileNameWithoutExtension(callerFile), callerMember);

            Directory.CreateDirectory(testDirectoryPath);

            string testFilePath = Path.Combine(testDirectoryPath, relativeFilePath.Insert(relativeFilePath.LastIndexOf("."), postfix));

            File.Copy(filePath, testFilePath);

            return testFilePath;
        }

    }
}
