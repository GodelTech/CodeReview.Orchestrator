using System.ComponentModel;
using System.IO;
using System.Linq;
using ByteSizeLib;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DirectoryService : IDirectoryService
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public ByteSize DirectorySize(string path)
        {
            var bytes = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                .Select(p => new FileInfo(p))
                .Sum(fileInfo => fileInfo.Length);

            return ByteSize.FromBytes(bytes);
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }
    }
}