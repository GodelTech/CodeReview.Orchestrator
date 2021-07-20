using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class RunProcessorsActivity : IActivity
    {
        private readonly AnalysisManifest _analysisManifest;
        private readonly IActivityExecutor _activityExecutor;
        private readonly ILogger<RunProcessorsActivity> _logger;

        public RunProcessorsActivity(
            AnalysisManifest analysisManifest,
            IActivityExecutor activityExecutor,
            ILogger<RunProcessorsActivity> logger)
        {
            _analysisManifest = analysisManifest ?? throw new ArgumentNullException(nameof(analysisManifest));
            _activityExecutor = activityExecutor ?? throw new ArgumentNullException(nameof(activityExecutor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ExecuteAsync(IProcessingContext context)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));

            foreach (var (activityName, manifest) in _analysisManifest.Activities)
            {
                _logger.LogInformation("Running activity: {activityName}", activityName);

                OverrideActivityVolumes(manifest);
                
                try
                {
                    var result = await _activityExecutor.ExecuteAsync(
                        manifest,
                        true,
                        context);

                    if (result.ExitCode != Constants.SuccessExitCode)
                        return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Execution failed");
                    return false;
                }
            }

            return true;
        }

        //TODO:
        private void OverrideActivityVolumes(ActivityManifest activityManifest)
        {
            foreach (var (volumeName, volume) in _analysisManifest.Volumes)
            {
                if (activityManifest.Volumes.TryGetValue(volumeName, out var activityVolume))
                {
                    
                }
                else
                {
                    activityManifest.Volumes.Add(volumeName, volume);
                }
            }
        }
    }
}