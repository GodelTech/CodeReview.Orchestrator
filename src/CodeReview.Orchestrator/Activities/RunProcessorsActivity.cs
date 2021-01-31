using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class RunProcessorsActivity : IActivity
    {
        private readonly IReadOnlyDictionary<string, ActivityManifest> _activities;
        private readonly IActivityExecutor _activityExecutor;
        private readonly ILogger<RunProcessorsActivity> _logger;

        public RunProcessorsActivity(
            IReadOnlyDictionary<string, ActivityManifest> activities,
            IActivityExecutor activityExecutor,
            ILogger<RunProcessorsActivity> logger)
        {
            _activities = activities ?? throw new ArgumentNullException(nameof(activities));
            _activityExecutor = activityExecutor ?? throw new ArgumentNullException(nameof(activityExecutor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ExecuteAsync(IProcessingContext context)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));

            foreach (var (activityName, manifest) in _activities)
            {
                _logger.LogInformation("Running activity: {activityName}", activityName);

                try
                {
                    var result = await _activityExecutor.ExecuteAsync(
                        manifest,
                        true,
                        context.SourceCodeVolumeId,
                        context.ArtifactsVolumeId,
                        context.ImportsVolumeId);

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
    }
}
