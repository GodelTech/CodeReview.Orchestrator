using System;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ImportSourcesActivity : ImportFolderActivity
    {
        private readonly SourcesDataSettings _settings;
        
        protected override string ContainerFolderPath => "/src";
        protected override string HostFolderPath => _settings.FolderPath;

        public ImportSourcesActivity(
            SourcesDataSettings settings,
            IContainerService containerService,
            ITarArchiveService tarArchiveService,
            IDirectoryService directoryService,
            ILogger logger)
            : base(containerService, tarArchiveService, directoryService, logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        
        protected override string GetVolumeToMount(IProcessingContext context) => context.SourceCodeVolumeId;
    }
}