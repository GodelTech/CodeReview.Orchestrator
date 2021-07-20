using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ProcessingContext : IProcessingContext
    {
        private readonly string _manifestFilePath;
        private readonly IContainerService _containerService;
        private readonly IPathService _pathService;
        private readonly Dictionary<string, string> _volumes = new();

        public IReadOnlyDictionary<string, string> Volumes => _volumes;

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

        public async Task InitializeAsync(ICollection<string> volumes)
        {
            foreach (var volume in volumes)
            {
                var volumeId = await _containerService.CreateVolumeAsync(volume).ConfigureAwait(false);
                
                _volumes.Add(volume, volumeId);
            }
        }

        public string ResolvePath(string relativeOrAbsolutePath)
        {
            if (string.IsNullOrWhiteSpace(relativeOrAbsolutePath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(relativeOrAbsolutePath));

            if (_pathService.IsPathRooted(relativeOrAbsolutePath))
                return relativeOrAbsolutePath;
            
            var manifestFolderPath = _pathService.GetDirectoryName(_manifestFilePath);
            var importsFolderPath = _pathService.Combine(manifestFolderPath, relativeOrAbsolutePath);
            
            return  _pathService.GetFullPath(importsFolderPath);
        }

        public async Task CleanUpVolumesAsync()
        {
            foreach (var (_, volumeId) in _volumes)
            {
                await _containerService.RemoveVolumeAsync(volumeId);
            }
            
            _volumes.Clear();
        }

        public async ValueTask DisposeAsync()
        {
            await CleanUpVolumesAsync();
        }
    }
}