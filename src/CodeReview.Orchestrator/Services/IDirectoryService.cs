using System.IO;
using GodelTech.CodeReview.Orchestrator.Utils;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IDirectoryService
    {
        ByteSize GetDirectorySize(string path);
        bool Exists(string path);
        DirectoryInfo CreateDirectory(string path);
        void Delete(string path, bool recursive);
    }
}