using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerContextSwitcher : IDockerContextSwitcher
    {
        private readonly IDockerEngineContext _engineContext;
        private readonly IDockerEngineProvider _engineProvider;
        private readonly ITempFolderFactory _tempFolderFactory;
        private readonly IContainerService _containerService;
        private readonly ITarArchiveService _tarArchiveService;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;

        public DockerContextSwitcher(
            IDockerEngineContext engineContext,
            IDockerEngineProvider engineProvider,
            ITempFolderFactory tempFolderFactory,
            IContainerService containerService,
            ITarArchiveService tarArchiveService,
            IDirectoryService directoryService,
            IPathService pathService)
        {
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
            _engineProvider = engineProvider ?? throw new ArgumentNullException(nameof(engineProvider));
            _tempFolderFactory = tempFolderFactory ?? throw new ArgumentNullException(nameof(tempFolderFactory));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _tarArchiveService = tarArchiveService ?? throw new ArgumentNullException(nameof(tarArchiveService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public async Task SwitchAsync(IProcessingContext context, RequirementsManifest requirements, IReadOnlyDictionary<string, Volume> volumesToCopy)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));

            var matchingEngine = _engineProvider.Find(requirements);

            if (_engineContext.Engine == null)
            {
                _engineContext.Engine = matchingEngine;
                return;
            }

            if (_engineContext.Engine.Equals(matchingEngine))
                return;
            
            await SwitchVolumesAsync(context, matchingEngine, volumesToCopy);
        }

        private async Task SwitchVolumesAsync(IProcessingContext context, DockerEngine newEngine, IReadOnlyDictionary<string, Volume> volumesToCopy)
        {
            using var folder = _tempFolderFactory.Create();

            var containerId = await CreateWorkerContainerAsync(context, volumesToCopy);

            try
            {
                foreach (var (volumeName, volume) in volumesToCopy)
                {
                    var volumeHostFolder = _pathService.Combine(folder.Path, volumeName);
                    var volumeContainerFolder = volume.TargetFolder;

                    await ExportFolderContent(containerId, volumeContainerFolder, volumeHostFolder);
                }
            }
            finally
            {
                await _containerService.RemoveContainerAsync(containerId);
            }

            await context.CleanUpVolumesAsync();

            _engineContext.Engine = newEngine;

            await context.InitializeAsync(volumesToCopy.Keys.ToArray());

            containerId = await CreateWorkerContainerAsync(context, volumesToCopy);

            try
            {
                foreach (var (volumeName, volume) in volumesToCopy)
                {
                    var volumeHostFolder = _pathService.Combine(folder.Path, volumeName);
                    var volumeContainerFolder = volume.TargetFolder;

                    await ImportDataToContainerAsync(volumeHostFolder, containerId, volumeContainerFolder);
                }
            }
            finally
            {
                await _containerService.RemoveContainerAsync(containerId);
            }
        }

        private async Task ImportDataToContainerAsync(string dataFolder, string containerId, string containerFolder)
        {
            if (!_directoryService.Exists(dataFolder))
                return;
            
            await using var outStream = _tarArchiveService.Create(dataFolder);

            await _containerService.ImportFilesIntoContainerAsync(containerId, containerFolder, outStream);
        }

        private async Task<string> CreateWorkerContainerAsync(IProcessingContext context, IReadOnlyDictionary<string, Volume> volumes)
        {
            var mountedVolumes = ResolveMountedVolumes(context, volumes).ToArray();
            
            return await _containerService.CreateContainerAsync(_engineContext.Engine.WorkerImage, mountedVolumes);
        }

        private async Task ExportFolderContent(string containerId, string containerFolder, string hostFolderPath)
        {
            await using var outStream = new MemoryStream();
            
            await _containerService.ExportFilesFromContainerAsync(containerId, containerFolder, outStream);

            outStream.Position = 0;

            _tarArchiveService.Extract(outStream, hostFolderPath, containerFolder);
        }
        
        private static IEnumerable<MountedVolume> ResolveMountedVolumes(IProcessingContext context, IReadOnlyDictionary<string, Volume> volumes)
        {
            return volumes.Select(pair => new MountedVolume
            {
                IsBindMount = false,
                Source = context.Volumes[pair.Key],
                Target = pair.Value.TargetFolder,
                IsReadOnly = pair.Value.ReadOnly
            });
        }
    }
}