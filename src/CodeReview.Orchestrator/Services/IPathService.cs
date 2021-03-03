namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IPathService
    {
        string GetDirectoryName(string path);
        string GetFullPath(string path);
        string Combine(params string[] parts);
        bool IsPathRooted(string path);
    }
}