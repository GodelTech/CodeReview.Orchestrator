using System;
using System.Collections.Immutable;
using System.IO;
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

        public async Task SwitchAsync(IProcessingContext context, RequirementsManifest requirements)
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
            
            await SwitchVolumesAsync(context, matchingEngine);
        }

        private async Task SwitchVolumesAsync(IProcessingContext context, DockerEngine newEngine)
        {
            using var folder = _tempFolderFactory.Create();
            
            var artifactsFolder = _pathService.Combine(folder.Path, "artifacts");
            var srcFolder = _pathService.Combine(folder.Path, "src");
            var importsFolder = _pathService.Combine(folder.Path, "imports");

            var containerId = await CreateWorkerContainerAsync(context);

            try
            {
                await ExportFolderContent(containerId, GetArtifactsFolderForOs(_engineContext.Engine.Os), artifactsFolder);
                await ExportFolderContent(containerId, GetSrcFolderForOs(_engineContext.Engine.Os), srcFolder);
                await ExportFolderContent(containerId, GetImportsFolderForOs(_engineContext.Engine.Os), importsFolder);
            }
            finally
            {
                await _containerService.RemoveContainerAsync(containerId);
            }

            await context.CleanUpVolumesAsync();

            _engineContext.Engine = newEngine;

            await context.InitializeAsync();

            containerId = await CreateWorkerContainerAsync(context);

            try
            {
                await ImportDataToContainerAsync(artifactsFolder, containerId, GetArtifactsFolderForOs(_engineContext.Engine.Os));
                await ImportDataToContainerAsync(srcFolder, containerId, GetSrcFolderForOs(_engineContext.Engine.Os));
                await ImportDataToContainerAsync(importsFolder, containerId, GetImportsFolderForOs(_engineContext.Engine.Os));
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

        private async Task<string> CreateWorkerContainerAsync(IProcessingContext context)
        {
            return await _containerService.CreateContainerAsync(
                _engineContext.Engine.WorkerImage,
                Array.Empty<string>(),
                ImmutableDictionary<string, string>.Empty,
                new MountedVolume
                {
                    IsBindMount = false,
                    Source = context.ArtifactsVolumeId,
                    Target = GetArtifactsFolderForOs(_engineContext.Engine.Os)
                },
                new MountedVolume
                {
                    IsBindMount = false,
                    Source = context.SourceCodeVolumeId,
                    Target = GetSrcFolderForOs(_engineContext.Engine.Os)
                },
                new MountedVolume
                {
                    IsBindMount = false,
                    Source = context.ImportsVolumeId,
                    Target = GetImportsFolderForOs(_engineContext.Engine.Os)
                });
        }

        private async Task ExportFolderContent(string containerId, string containerFolder, string hostFolderPath)
        {
            await using var outStream = new MemoryStream();
            
            await _containerService.ExportFilesFromContainerAsync(containerId, containerFolder, outStream);

            outStream.Position = 0;

            _tarArchiveService.Extract(outStream, hostFolderPath, containerFolder);
        }

        private static string GetSrcFolderForOs(OperatingSystemType os)
        {
            return os == OperatingSystemType.Linux ? "/src" : "C:\\src";
        }

        private static string GetArtifactsFolderForOs(OperatingSystemType os)
        {
            return os == OperatingSystemType.Linux ? "/artifacts" : "C:\\artifacts";
        }

        private static string GetImportsFolderForOs(OperatingSystemType os)
        {
            return os == OperatingSystemType.Linux ? "/imports" : "C:\\imports";
        }
    }
}