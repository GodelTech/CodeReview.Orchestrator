using System.IO;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface ITarArchiveService
    {
        Stream CreateInMemory(string path);
        Stream CreateInFile(string path, string tmpFilePath);
        void Extract(Stream inStream, string folderPath, string pathToRemove);
    }
}