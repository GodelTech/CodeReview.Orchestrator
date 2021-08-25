using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerVolumeExporter : IDockerVolumeExporter
    {
        private readonly IContainerService _containerService;
        private readonly ITarArchiveService _tarArchiveService;
        private readonly IDockerEngineContext _dockerEngineContext;

        public DockerVolumeExporter(
            IContainerService containerService, 
            ITarArchiveService tarArchiveService, 
            IDockerEngineContext dockerEngineContext)
        {
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _tarArchiveService = tarArchiveService ?? throw new ArgumentNullException(nameof(tarArchiveService));
            _dockerEngineContext = dockerEngineContext ?? throw new ArgumentNullException(nameof(dockerEngineContext));
        }

        public async Task ExportVolumesAsync(IProcessingContext context, params Volume[] volumes)
        {
            var containerId = await CreateWorkerContainerAsync(context, volumes);

            try
            {
                foreach (var volume in volumes)
                {
                    var folderPath = context.ResolvePath(volume.FolderToOutput);
                    
                    await using var outStream = new MemoryStream();
                    
                    await _containerService.ExportFilesFromContainerAsync(containerId, volume.TargetFolder, outStream);
                    
                    outStream.Position = 0;

                    _tarArchiveService.Extract(outStream, folderPath, volume.TargetFolder);
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

            return await _containerService.CreateContainerAsync(_dockerEngineContext.Engine.WorkerImage, mountedVolumes);
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