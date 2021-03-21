using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ImportDataActivity : ImportFolderActivity
    {
        protected override string ContainerFolderPath => "/imports";
        protected override string HostFolderPath => _settings.FolderPath;

        private readonly ImportedDataSettings _settings;

        public ImportDataActivity(
            ImportedDataSettings importedDataSettings,
            IContainerService containerService,
            ITarArchiveService tarArchiveService,
            IDirectoryService directoryService,
            ILogger<ImportDataActivity> logger)
            : base(containerService, tarArchiveService, directoryService, logger)
        {
            _settings = importedDataSettings ?? throw new ArgumentNullException(nameof(importedDataSettings));
        }
    }
}