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
        private readonly IDockerEngineContext _engineContext;
        private readonly IContainerService _containerService;
        private readonly IActivityExecutor _activityExecutor;
        private readonly ITarArchiveService _archiveService;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly ILoggerFactory _loggerFactory;

        public ActivityFactory(
            IDockerEngineContext engineContext,
            IContainerService containerService,
            IActivityExecutor activityExecutor,
            ITarArchiveService archiveService,
            IDirectoryService directoryService,
            IPathService pathService,
            ILoggerFactory loggerFactory)
        {
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _activityExecutor = activityExecutor ?? throw new ArgumentNullException(nameof(activityExecutor));
            _archiveService = archiveService ?? throw new ArgumentNullException(nameof(archiveService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
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
            var volumes = manifest.Volumes.Where(p => !string.IsNullOrWhiteSpace(p.Value.FolderToImport));
            
            foreach (var (volumeName, volume) in volumes)
            {
                var logger = _loggerFactory.CreateLogger<ImportFolderActivity>();
                
                yield return new ImportFolderActivity(_engineContext, _containerService, _archiveService, _directoryService, logger)
                {
                    Volume = volumeName,
                    HostFolderPath = volume.FolderToImport,
                    ContainerFolderPath = volume.TargetFolder
                };
            }
        }

        private IEnumerable<IActivity> CreateRunProcessorsActivity(AnalysisManifest manifest)
        {
            var logger = _loggerFactory.CreateLogger<RunProcessorsActivity>();

            yield return new RunProcessorsActivity(manifest, _activityExecutor, logger);
        }
        
        private IEnumerable<IActivity> CreateExportActivities(AnalysisManifest manifest)
        {
            var volumes = manifest.Volumes.Where(p => !string.IsNullOrWhiteSpace(p.Value.FolderToOutput));
            
            foreach (var (volumeName, volume) in volumes)
            {
                var logger = _loggerFactory.CreateLogger<ExportFolderActivity>();
                
                yield return new ExportFolderActivity(_engineContext, _containerService, _archiveService, _directoryService, _pathService, logger)
                {
                    Volume = volumeName,
                    HostFolderPath = volume.FolderToOutput,
                    ContainerFolderPath = volume.TargetFolder
                };
            }
        }
    }
}
