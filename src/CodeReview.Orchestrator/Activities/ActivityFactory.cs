﻿using System;
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
            if (manifest == null) 
                throw new ArgumentNullException(nameof(manifest));

            return new CompositeActivity(
                new ImportDataActivity(
                    manifest.Imports,
                    _engineContext,
                    _containerService,
                    _archiveService,
                    _directoryService,
                    _loggerFactory.CreateLogger<ImportDataActivity>()),
                new ImportSourcesActivity(
                    manifest.Sources,
                    _engineContext,
                    _containerService,
                    _archiveService,
                    _directoryService,
                    _loggerFactory.CreateLogger<ImportSourcesActivity>()),
                new RunProcessorsActivity(
                    manifest.Activities, 
                    _activityExecutor,
                    _loggerFactory.CreateLogger<RunProcessorsActivity>()),
                new ExportArtifactsActivity(
                    manifest.Artifacts,
                    _engineContext,
                    _containerService,
                    _archiveService,
                    _directoryService,
                    _pathService,
                    _loggerFactory.CreateLogger<ExportArtifactsActivity>())
                );

        }
    }
}
