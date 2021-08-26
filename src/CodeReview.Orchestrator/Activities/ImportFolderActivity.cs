using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ImportFolderActivity : IActivity
    {
        private readonly IDockerVolumeImporter _dockerVolumeImporter;
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<ImportFolderActivity> _logger;

        public Volume Volume { get; init; }

        public ImportFolderActivity(
            IDockerVolumeImporter dockerVolumeImporter,
            IDirectoryService directoryService,
            ILogger<ImportFolderActivity> logger)
        {
            _dockerVolumeImporter = dockerVolumeImporter ?? throw new ArgumentNullException(nameof(dockerVolumeImporter));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> ExecuteAsync(IProcessingContext context)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));

            var folderPath = context.ResolvePath(Volume.FolderToImport);
            
            if (!_directoryService.Exists(folderPath))
            {
                _logger.LogInformation("Import folder not found. Import is not performed. Folder = {folderPath}", folderPath);
                return true;
            }

            _logger.LogInformation("Starting {VolumeName} import...", Volume.Name);
            await _dockerVolumeImporter.ImportVolumesAsync(context, Volume);
            _logger.LogInformation("{VolumeName} import completed.", Volume.Name);

            return true;
        }
    }
}