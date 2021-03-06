﻿using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ExportArtifactsActivity : IActivity
    {
        private const string ArtifactsFolderPath = "/artifacts";

        private readonly ArtifactsSettings _settings;
        private readonly IDockerEngineContext _engineContext;
        private readonly IContainerService _containerService;
        private readonly ITarArchiveService _tarArchiveService;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly ILogger<ExportArtifactsActivity> _logger;

        public ExportArtifactsActivity(
            ArtifactsSettings importedDataSettings,
            IDockerEngineContext engineContext,
            IContainerService containerService,
            ITarArchiveService tarArchiveService,
            IDirectoryService directoryService,
            IPathService pathService,
            ILogger<ExportArtifactsActivity> logger)
        {
            _settings = importedDataSettings ?? throw new ArgumentNullException(nameof(importedDataSettings));
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
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

            var folderPath = context.ResolvePath(_settings.FolderPath);

            if (!_directoryService.Exists(folderPath))
            {
                _logger.LogInformation("Creating directory for artifacts export. Folder = {folderPath}", _pathService.GetFullPath(folderPath));
                _directoryService.CreateDirectory(folderPath);
            }

            _logger.LogInformation("Starting artifacts export...");

            var containerId = await _containerService.CreateContainerAsync(
                _engineContext.Engine.WorkerImage,
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

                    _tarArchiveService.Extract(outStream, folderPath, ArtifactsFolderPath);
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