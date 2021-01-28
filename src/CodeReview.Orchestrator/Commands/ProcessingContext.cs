using System;
using System.Threading.Tasks;
using ReviewItEasy.Orchestrator.Services;

namespace ReviewItEasy.Orchestrator.Commands
{
    public class ProcessingContext : IProcessingContext
    {
        private readonly IContainerService _containerService;

        public string SourceCodeVolumeId { get; private set; }
        public string ArtifactsVolumeId { get; private set; }
        public string ImportsVolumeId { get; private set; }

        public ProcessingContext(IContainerService containerService)
        {
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
        }

        public async Task InitializeAsync()
        {
            SourceCodeVolumeId = await _containerService.CreateVolumeAsync("src");
            ArtifactsVolumeId = await _containerService.CreateVolumeAsync("art");
            ImportsVolumeId = await _containerService.CreateVolumeAsync("imp");
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