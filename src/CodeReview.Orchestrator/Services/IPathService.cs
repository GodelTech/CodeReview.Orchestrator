namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IPathService
    {
        string GetDirectoryName(string path);
        string GetFullPath(string path);
        string GetTempFileName();
        string Combine(params string[] parts);
        string ChangeExtensions(string path, string ext);
        bool IsPathRooted(string path);
        string GetTempPath();
    }
}