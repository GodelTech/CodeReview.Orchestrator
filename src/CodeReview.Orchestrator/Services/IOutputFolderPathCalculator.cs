namespace ReviewItEasy.Orchestrator.Services
{
    public interface IOutputFolderPathCalculator
    {
        string CalculateOutputDirectoryName(string dstRootFolderPath, string originalFolderPath, string archiveEntryPath);
    }
}