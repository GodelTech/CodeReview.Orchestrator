using System.IO;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class PathService : IPathService
    {
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