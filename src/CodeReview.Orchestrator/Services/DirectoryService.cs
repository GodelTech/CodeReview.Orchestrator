using System.IO;

namespace ReviewItEasy.Orchestrator.Services
{
    public class DirectoryService : IDirectoryService
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public string[] GetFiles(string path, string pattern, SearchOption options)
        {
            return Directory.GetFiles(path, pattern, options);
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}