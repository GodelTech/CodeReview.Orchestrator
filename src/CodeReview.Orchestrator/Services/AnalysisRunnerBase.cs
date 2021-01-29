using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public abstract class AnalysisRunnerBase
    {
        protected IAnalysisManifestProvider AnalysisManifestProvider { get; }
        protected IManifestValidator ManifestValidator { get; }
        protected IManifestExpressionExpander ManifestExpressionExpander { get; }
        protected ILogger<AnalysisRunnerBase> Logger { get; }

        protected AnalysisRunnerBase(
            IAnalysisManifestProvider analysisManifestProvider,
            IManifestValidator manifestValidator,
            IManifestExpressionExpander manifestExpressionExpander,
            ILogger<AnalysisRunnerBase> logger)
        {
            AnalysisManifestProvider = analysisManifestProvider ?? throw new ArgumentNullException(nameof(analysisManifestProvider));
            ManifestValidator = manifestValidator ?? throw new ArgumentNullException(nameof(manifestValidator));
            ManifestExpressionExpander = manifestExpressionExpander ?? throw new ArgumentNullException(nameof(manifestExpressionExpander));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected  async Task<AnalysisManifest> GetManifestAsync(string manifestPath)
        {
            if (string.IsNullOrWhiteSpace(manifestPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(manifestPath));

            Logger.LogInformation("Reading and deserializing manifest...");

            var manifest = await AnalysisManifestProvider.GetAsync(manifestPath);
            if (manifest == null)
            {
                Logger.LogError("Failed to read manifest");
                return null;
            }

            Logger.LogInformation("Manifest deserialized");
            Logger.LogInformation("Validating manifest...");

            if (!ManifestValidator.IsValid(manifest))
            {
                Logger.LogInformation("Manifest validation failed");
                return null;
            }

            Logger.LogInformation("Manifest validated");

            Logger.LogInformation("Resolving manifest variables...");
            ManifestExpressionExpander.Expand(manifest);
            Logger.LogInformation("Manifest variables resolved");

            return manifest;
        }
    }
}