using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerVolumeImporter : IDockerVolumeImporter
    {
        private readonly IContainerService _containerService;
        private readonly IDirectoryService _directoryService;
        private readonly ITarArchiveService _tarArchiveService;
        private readonly IDockerEngineContext _dockerEngineContext;

        public DockerVolumeImporter(
            IContainerService containerService, 
            IDirectoryService directoryService, 
            ITarArchiveService tarArchiveService, 
            IDockerEngineContext dockerEngineContext)
        {
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _tarArchiveService = tarArchiveService ?? throw new ArgumentNullException(nameof(tarArchiveService));
            _dockerEngineContext = dockerEngineContext ?? throw new ArgumentNullException(nameof(dockerEngineContext));
        }

        public async Task ImportVolumesAsync(IProcessingContext context, params Volume[] volumes)
        {
            var containerId = await CreateWorkerContainerAsync(context, volumes);

            try
            {
                foreach (var volume in volumes)
                {
                    var folderPath = context.ResolvePath(volume.FolderToImport);
                    
                    if (!_directoryService.Exists(folderPath))
                        continue;

                    await using var outStream = _tarArchiveService.Create(folderPath);

                    await _containerService.ImportFilesIntoContainerAsync(containerId, volume.TargetFolder, outStream);
                }
            }
            finally
            {
                await _containerService.RemoveContainerAsync(containerId);
            }
        }
        
        private async Task<string> CreateWorkerContainerAsync(IProcessingContext context, ICollection<Volume> volumes)
        {
            var mountedVolumes = ResolveMountedVolumes(context, volumes).ToArray();

            return await _containerService.CreateContainerAsync(_dockerEngineContext.Engine.WorkerImage, _dockerEngineContext.Engine.ResourceLabel, mountedVolumes);
        }

        private static IEnumerable<MountedVolume> ResolveMountedVolumes(IProcessingContext context, ICollection<Volume> volumes)
        {
            return volumes.Select(volume => new MountedVolume
            {
                IsBindMount = false,
                Source = context.ResolveVolumeId(volume.Name),
                Target = volume.TargetFolder
            });
        }
    }
}