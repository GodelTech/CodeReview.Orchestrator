using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ImportFolderActivity : IActivity
    {
        private readonly IDockerEngineContext _engineContext;
        private readonly IContainerService _containerService;
        private readonly ITarArchiveService _tarArchiveService;
        private readonly IDirectoryService _directoryService;
        private readonly ILogger _logger;

        public string ContainerFolderPath { get; init; }
        public string HostFolderPath { get; init; }
        public string Volume { get; init; }

        public ImportFolderActivity(
            IDockerEngineContext engineContext,
            IContainerService containerService,
            ITarArchiveService tarArchiveService,
            IDirectoryService directoryService,
            ILogger logger)
        {
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _tarArchiveService = tarArchiveService ?? throw new ArgumentNullException(nameof(tarArchiveService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> ExecuteAsync(IProcessingContext context)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));

            var folderPath = context.ResolvePath(HostFolderPath);
            
            if (!_directoryService.Exists(folderPath))
            {
                _logger.LogInformation("No sources folder not found. Import is not performed. Folder = {folderPath}", folderPath);
                return true;
            }

            _logger.LogInformation("Starting sources import...");

            var containerId = await _containerService.CreateContainerAsync(
                _engineContext.Engine.WorkerImage,
                Array.Empty<string>(),
                ImmutableDictionary<string, string>.Empty,
                new MountedVolume
                {
                    IsBindMount = false,
                    Source = context.Volumes[Volume],
                    Target = ContainerFolderPath
                });

            try
            {
                await using var outStream = _tarArchiveService.Create(folderPath);

                await _containerService.ImportFilesIntoContainerAsync(containerId, ContainerFolderPath, outStream);

                _logger.LogInformation("Sources import completed.");
            }
            finally
            {
                await _containerService.RemoveContainerAsync(containerId);
            }

            return true;
        }
    }
}