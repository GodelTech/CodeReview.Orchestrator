using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReviewItEasy.Orchestrator.Model;
using ReviewItEasy.Orchestrator.Services;

namespace ReviewItEasy.Orchestrator.Commands
{
    public class ExportArtifactsActivity : IActivity
    {
        private const string ArtifactsFolderPath = "/artifacts";

        private readonly ArtifactsSettings _settings;
        private readonly IContainerService _containerService;
        private readonly ITarArchiveService _tarArchiveService;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly ILogger<ExportArtifactsActivity> _logger;

        public ExportArtifactsActivity(
            ArtifactsSettings importedDataSettings,
            IContainerService containerService,
            ITarArchiveService tarArchiveService,
            IDirectoryService directoryService,
            IPathService pathService,
            ILogger<ExportArtifactsActivity> logger)
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

            if (!_settings.ExportOnCompletion)
            {
                _logger.LogInformation("Artifacts export is not required.");
                return true;
            }


            if (!_directoryService.Exists(_settings.FolderPath))
            {
                _logger.LogInformation("Creating directory for artifacts export. Folder = {folderPath}", _pathService.GetFullPath(_settings.FolderPath));
                _directoryService.CreateDirectory(_settings.FolderPath);
            }

            _logger.LogInformation("Starting artifacts export...");

            var containerId = await _containerService.CreateContainerAsync(
                Constants.WorkerImage,
                Array.Empty<string>(),
                ImmutableDictionary<string, string>.Empty,
                new MountedVolume
                {
                    IsBindMount = false,
                    Source = context.ArtifactsVolumeId,
                    Target = ArtifactsFolderPath
                });

            try
            {
                await using (var outStream = new MemoryStream())
                {
                    await _containerService.ExportFilesFromContainerAsync(containerId, ArtifactsFolderPath, outStream);

                    outStream.Position = 0;

                    _tarArchiveService.Extract(outStream, _settings.FolderPath, ArtifactsFolderPath);
                }

                _logger.LogInformation("Artifacts export completed.");
            }
            finally
            {
                await _containerService.RemoveContainerAsync(containerId);
            }

            return true;
        }
    }
}