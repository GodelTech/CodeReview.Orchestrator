using System;

namespace ReviewItEasy.Orchestrator.Services
{
    public class OutputFolderPathCalculator : IOutputFolderPathCalculator
    {
        private readonly IPathService _pathService;

        public OutputFolderPathCalculator(IPathService pathService)
        {
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }
        
        public string CalculateOutputDirectoryName(string dstRootFolderPath, string originalFolderPath, string archiveEntryPath)
        {
            if (dstRootFolderPath == null) 
                throw new ArgumentNullException(nameof(dstRootFolderPath));
            if (originalFolderPath == null) 
                throw new ArgumentNullException(nameof(originalFolderPath));
            if (archiveEntryPath == null) 
                throw new ArgumentNullException(nameof(archiveEntryPath));
            
            if (originalFolderPath.StartsWith("/"))
                originalFolderPath = originalFolderPath[1..];

            if (originalFolderPath.EndsWith("/"))
                originalFolderPath = originalFolderPath[..^1];

            var fullPath = _pathService.GetFullPath(dstRootFolderPath);
            var entryDirectoryName = _pathService.GetDirectoryName(archiveEntryPath);

            var relativePath = entryDirectoryName[originalFolderPath.Length..];
            if (string.IsNullOrWhiteSpace(relativePath))
                return fullPath.Replace("\\", "/");

            if (relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
                relativePath = relativePath[1..];
            
            return _pathService.Combine(fullPath, relativePath).Replace("\\", "/");
        }
    }
}