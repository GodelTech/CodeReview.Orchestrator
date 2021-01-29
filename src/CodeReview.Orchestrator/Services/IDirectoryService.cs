using System.IO;

namespace CodeReview.Orchestrator.Services
{
    public interface IDirectoryService
    {
        bool Exists(string path);
        string[] GetFiles(string path, string pattern, SearchOption options);
        DirectoryInfo CreateDirectory(string path);
    }
}