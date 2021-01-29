using System.IO;

namespace CodeReview.Orchestrator.Services
{
    public interface ITarArchiveService
    {
        Stream Create(string path);
        void Extract(Stream inStream, string folderPath, string pathToRemove);
    }
}