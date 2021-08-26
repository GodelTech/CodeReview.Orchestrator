using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ExportFolderActivity : IActivity
    {
        private readonly IDockerVolumeExporter _dockerVolumeExporter;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly ILogger<ExportFolderActivity> _logger;
        
        public Volume Volume { get; init; }
        
        public ExportFolderActivity(
            IDockerVolumeExporter dockerVolumeExporter,
            IDirectoryService directoryService,
            IPathService pathService,
            ILogger<ExportFolderActivity> logger)
        {
            _dockerVolumeExporter = dockerVolumeExporter ?? throw new ArgumentNullException(nameof(dockerVolumeExporter));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ExecuteAsync(IProcessingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var folderPath = context.ResolvePath(Volume.FolderToOutput);

            if (!_directoryService.Exists(folderPath))
            {
                _logger.LogInformation("Creating directory for {VolumeName} export. Folder = {folderPath}", Volume.Name, _pathService.GetFullPath(folderPath));
                _directoryService.CreateDirectory(folderPath);
            }

            _logger.LogInformation("Starting {VolumeName} export...", Volume.Name);
            
            await _dockerVolumeExporter.ExportVolumesAsync(context, Volume);
            
            _logger.LogInformation("{VolumeName} export completed.", Volume.Name);

            return true;
        }
    }
}