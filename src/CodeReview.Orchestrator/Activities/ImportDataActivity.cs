using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ImportDataActivity : IActivity
    {
        private const string ImportsFolderPath = "/imports";
        
        private readonly ImportedDataSettings _settings;
        private readonly IContainerService _containerService;
        private readonly ITarArchiveService _tarArchiveService;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly ILogger<ImportDataActivity> _logger;

        public ImportDataActivity(
            ImportedDataSettings importedDataSettings,
            IContainerService containerService,
            ITarArchiveService tarArchiveService,
            IDirectoryService directoryService,
            IPathService pathService,
            ILogger<ImportDataActivity> logger)
        {
            _settings = importedDataSettings ?? throw new ArgumentNullException(nameof(importedDataSettings));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _tarArchiveService = tarArchiveService ?? throw new ArgumentNullException(nameof(tarArchiveService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> ExecuteAsync(IProcessingContext context)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));

            var folderPath = context.ResolvePath(_settings.FolderPath);
            
            if (!_directoryService.Exists(folderPath))
            {
                _logger.LogInformation("No imports folder found. Import is not performed. Folder = {folderPath}", folderPath);
                return true;
            }

            _logger.LogInformation("Starting data import...");

            var containerId = await _containerService.CreateContainerAsync(
                Constants.WorkerImage,
                Array.Empty<string>(),
                ImmutableDictionary<string, string>.Empty,
                new MountedVolume
                {
                    IsBindMount = false,
                    Source = context.ImportsVolumeId,
                    Target = ImportsFolderPath
                });

            try
            {
                await using var outStream = _tarArchiveService.Create(folderPath);

                await _containerService.ImportFilesIntoContainerAsync(containerId, ImportsFolderPath, outStream);

                _logger.LogInformation("Data import completed.");
            }
            finally
            {
                await _containerService.RemoveContainerAsync(containerId);
            }

            return true;
        }
    }
}