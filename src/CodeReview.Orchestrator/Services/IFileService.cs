using System.IO;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IFileService
    {
        void Delete(string path);
        Stream Open(string path);
        void Move(string sourceFileName, string destFileName);
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string text);
        bool Exists(string path);
    }
}