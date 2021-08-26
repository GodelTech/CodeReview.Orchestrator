using System;
using System.Collections.Generic;
using System.Linq;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ActivityFactory : IActivityFactory
    {
        private readonly IDockerVolumeExporter _dockerVolumeExporter;
        private readonly IDockerVolumeImporter _dockerVolumeImporter;
        private readonly IActivityExecutor _activityExecutor;
        private readonly IDirectoryService _directoryService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPathService _pathService;

        public ActivityFactory(
            IDockerVolumeExporter dockerVolumeExporter, 
            IDockerVolumeImporter dockerVolumeImporter, 
            IActivityExecutor activityExecutor, 
            IDirectoryService directoryService,
            ILoggerFactory loggerFactory, 
            IPathService pathService)
        {
            _dockerVolumeExporter = dockerVolumeExporter ?? throw new ArgumentNullException(nameof(dockerVolumeExporter));
            _dockerVolumeImporter = dockerVolumeImporter ?? throw new ArgumentNullException(nameof(dockerVolumeImporter));
            _activityExecutor = activityExecutor ?? throw new ArgumentNullException(nameof(activityExecutor));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public IActivity Create(AnalysisManifest manifest)
        {
            if (manifest == null) throw new ArgumentNullException(nameof(manifest));

            var activities = Enumerable.Empty<IActivity>()
                .Concat(CreateImportActivities(manifest))
                .Concat(CreateRunProcessorsActivity(manifest))
                .Concat(CreateExportActivities(manifest))
                .ToArray();
            
            return new CompositeActivity(activities);
        }

        private IEnumerable<IActivity> CreateImportActivities(AnalysisManifest manifest)
        {
            return manifest.Volumes
                .ListVolumesToImport()
                .Select(volume =>
                {
                    var logger = _loggerFactory.CreateLogger<ImportFolderActivity>();

                    return new ImportFolderActivity(_dockerVolumeImporter, _directoryService, logger)
                    {
                        Volume = volume
                    };
                });
        }

        private IEnumerable<IActivity> CreateRunProcessorsActivity(AnalysisManifest manifest)
        {
            var logger = _loggerFactory.CreateLogger<RunProcessorsActivity>();

            yield return new RunProcessorsActivity(manifest.Activities, _activityExecutor, logger);
        }
        
        private IEnumerable<IActivity> CreateExportActivities(AnalysisManifest manifest)
        {
            return manifest.Volumes
                .ListVolumesToExport()
                .Select(volume =>
                {
                    var logger = _loggerFactory.CreateLogger<ExportFolderActivity>();

                    return new ExportFolderActivity(_dockerVolumeExporter, _directoryService, _pathService, logger)
                    {
                        Volume = volume
                    };
                });
        }
    }
}
