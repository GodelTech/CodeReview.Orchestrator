using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ProcessingContext : IProcessingContext
    {
        private readonly string _manifestFilePath;
        private readonly IContainerService _containerService;
        private readonly IPathService _pathService;

        public string SourceCodeVolumeId { get; private set; }
        public string ArtifactsVolumeId { get; private set; }
        public string ImportsVolumeId { get; private set; }

        public ProcessingContext(
            string manifestFilePath,
            IContainerService containerService,
            IPathService pathService)
        {
            if (string.IsNullOrWhiteSpace(manifestFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(manifestFilePath));

            _manifestFilePath = manifestFilePath;
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public async Task InitializeAsync()
        {
            SourceCodeVolumeId = await _containerService.CreateVolumeAsync("src");
            ArtifactsVolumeId = await _containerService.CreateVolumeAsync("art");
            ImportsVolumeId = await _containerService.CreateVolumeAsync("imp");
        }

        public string ResolvePath(string relativeOrAbsolutePath)
        {
            if (string.IsNullOrWhiteSpace(relativeOrAbsolutePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(relativeOrAbsolutePath));

            if (_pathService.IsPathRooted(relativeOrAbsolutePath))
                return relativeOrAbsolutePath;
            
            var manifestFolderPath = _pathService.GetDirectoryName(_manifestFilePath);
            var importsFolderPath = _pathService.Combine(manifestFolderPath, relativeOrAbsolutePath);
            
            return  _pathService.GetFullPath(importsFolderPath);
        }

        public async ValueTask DisposeAsync()
        {
            if (!string.IsNullOrEmpty(SourceCodeVolumeId))
                await _containerService.RemoveVolumeAsync(SourceCodeVolumeId);

            if (!string.IsNullOrEmpty(ArtifactsVolumeId))
                await _containerService.RemoveVolumeAsync(ArtifactsVolumeId);

            if (!string.IsNullOrEmpty(ImportsVolumeId))
                await _containerService.RemoveVolumeAsync(ImportsVolumeId);
        }
    }
}