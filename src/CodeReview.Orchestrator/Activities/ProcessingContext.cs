using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ProcessingContext : IProcessingContext
    {
        private readonly string _manifestFilePath;
        private readonly IContainerService _containerService;
        private readonly IPathService _pathService;
        private readonly IDockerEngineContext _dockerEngineContext;
        private readonly Dictionary<string, Volume> _volumes = new(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<Volume> Volumes => _volumes.Values;
        
        public ProcessingContext(
            string manifestFilePath,
            IContainerService containerService,
            IPathService pathService,
            IDockerEngineContext dockerEngineContext)
        {
            if (string.IsNullOrWhiteSpace(manifestFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(manifestFilePath));

            _manifestFilePath = manifestFilePath;
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _dockerEngineContext = dockerEngineContext ?? throw new ArgumentNullException(nameof(dockerEngineContext));
        }

        public async Task InitializeAsync(ICollection<Volume> volumes)
        {
            if (volumes == null) throw new ArgumentNullException(nameof(volumes));
            
            foreach (var volume in volumes)
            {
                var volumeId = await _containerService.CreateVolumeAsync(volume.Name, _dockerEngineContext.Engine.ResourceLabel).ConfigureAwait(false);
                
                _volumes.Add(volumeId, volume);
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

        public string ResolveVolumeId(string volumeName)
        {
            if (string.IsNullOrWhiteSpace(volumeName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(volumeName));
            
            return _volumes.Single(p => p.Value.Name.Equals(volumeName, StringComparison.OrdinalIgnoreCase)).Key;
        }

        public async Task CleanUpVolumesAsync()
        {
            foreach (var (volumeId, _) in _volumes)
            {
                await _containerService.RemoveVolumeAsync(volumeId).ConfigureAwait(false);
            }
            
            _volumes.Clear();
        }

        public async ValueTask DisposeAsync()
        {
            await CleanUpVolumesAsync();
        }
    }
}