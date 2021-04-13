using System.IO;

namespace CodeReview.Orchestrator.SubsystemTests.Utils
{
    public static class FileHelper
    {
        public static string GetOutputPath(string fileName)
        {
            return Path.Combine(Config.OutputDirectoryPath, fileName);
        }

        public static void CopyFromResource(string resourceFileName, string filePath)
        {
            Directory.CreateDirectory(Config.OutputDirectoryPath);
            
            using var stream = Config.Assembly.GetManifestResourceStream(Config.ResourcePath + '.' + resourceFileName);
            using var fileStream = File.Create(filePath);
            
            stream.CopyTo(fileStream);
        }

        public static void CreateDirectory(string path)
        {
            var dirPath = Path.Combine(Config.OutputDirectoryPath, path);

            Directory.CreateDirectory(dirPath);
        }
    }
}