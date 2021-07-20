using System;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class RunAnalysisCommand : AnalysisRunnerBase, IRunAnalysisCommand
    {
        private readonly IDockerEngineContext _engineContext;
        private readonly IDockerEngineProvider _engineProvider;
        private readonly IActivityFactory _activityFactory;
        private readonly IProcessingContextFactory _processingContextFactory;
        private readonly IPathService _pathService;
        private readonly IDockerImageLoader _imageLoader;

        public RunAnalysisCommand(
            IDockerEngineContext engineContext,
            IDockerEngineProvider engineProvider,
            IManifestProvider analysisManifestProvider,
            IManifestValidator manifestValidator,
            IManifestExpressionExpander manifestExpressionExpander,
            IActivityFactory activityFactory,
            IProcessingContextFactory processingContextFactory,
            IPathService pathService,
            IDockerImageLoader imageLoader,
            ILogger<AnalysisRunnerBase> logger)
            : base(analysisManifestProvider, manifestValidator, manifestExpressionExpander, logger)
        {
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
            _engineProvider = engineProvider ?? throw new ArgumentNullException(nameof(engineProvider));
            _activityFactory = activityFactory ?? throw new ArgumentNullException(nameof(activityFactory));
            _processingContextFactory = processingContextFactory ?? throw new ArgumentNullException(nameof(processingContextFactory));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _imageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
        }

        public async Task<int> ExecuteAsync(RunOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));
            
            if (string.IsNullOrWhiteSpace(options.File))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(options.File));

            await _engineProvider.InitializeAsync(options.DockerEngineCollectionFilePath);
            
            var manifest = await GetManifestAsync(options.File);
            if (manifest == null)
                return Constants.ErrorExitCode;

            if (!manifest.Activities.Any())
            {
                Logger.LogWarning("No activities defined in manifest. Processing stopped.");
                return Constants.ErrorExitCode;
            }

            await _imageLoader.LoadImagesAsync(options.LoadBehavior, manifest);

            if (options.ExitAfterImageDownload)
                return Constants.SuccessExitCode;
            
            Logger.LogInformation("Preparing Docker Volumes...");

            await using var context = _processingContextFactory.Create(
                _pathService.GetFullPath(options.File));

            var engine = _engineProvider.Find(manifest.Activities.First().Value.Requirements);
            if (engine == null)
            {
                Logger.LogError("No matching Docker Engine found for first manifest activity.");
                return Constants.ErrorExitCode;
            }

            _engineContext.Engine = engine;
            
            await context.InitializeAsync(manifest.Volumes.ListVolumes());
            Logger.LogInformation("Docker Volumes created");

            var activity = _activityFactory.Create(manifest);

            Logger.LogInformation("Processing started.");
            var isSuccess = await activity.ExecuteAsync(context);
            Logger.LogInformation("Processing completed: {executionResult}", isSuccess ? "Success" : "Failure");

            return isSuccess ? Constants.SuccessExitCode : Constants.ErrorExitCode;
        }
    }
}