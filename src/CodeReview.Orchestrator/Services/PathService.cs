using System.IO;

namespace CodeReview.Orchestrator.Services
{
    public class PathService : IPathService
    {
        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }
        
        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public string Combine(params string[] parts)
        {
            return Path.Combine(parts);
        }
    }
}