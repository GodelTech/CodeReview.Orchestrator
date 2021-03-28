using System.IO;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IDirectoryService
    {
        bool Exists(string path);
        DirectoryInfo CreateDirectory(string path);
        void Delete(string path, bool recursive);
    }
}