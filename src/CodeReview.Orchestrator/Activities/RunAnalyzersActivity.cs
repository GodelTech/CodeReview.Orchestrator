using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class RunAnalyzersActivity : IActivity
    {
        private readonly string _workerImageName;
        private readonly IReadOnlyDictionary<string, ActivityManifest> _manifestAnalyzers;
        private readonly IContainerService _containerService;
        private readonly IActivityExecutor _activityExecutor;
        private readonly ILogger<RunAnalyzersActivity> _logger;

        private readonly ConcurrentBag<string> _volumesToCleanUp = new();
        
        public RunAnalyzersActivity(
            string workerImageName, 
            IReadOnlyDictionary<string, ActivityManifest> manifestAnalyzers, 
            IContainerService containerService,
            IActivityExecutor activityExecutor,
            ILogger<RunAnalyzersActivity> logger)
        {
            _workerImageName = workerImageName ?? throw new ArgumentNullException(nameof(workerImageName));
            _manifestAnalyzers = manifestAnalyzers ?? throw new ArgumentNullException(nameof(manifestAnalyzers));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _activityExecutor = activityExecutor ?? throw new ArgumentNullException(nameof(activityExecutor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ExecuteAsync(IProcessingContext context)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));

            _logger.LogInformation("Starting activities: {@activities}", _manifestAnalyzers.Keys.ToArray());

            try
            {
                var analyzerContainerTasks =
                    (from analyzer in _manifestAnalyzers
                        select RunAnalyzerAsync(analyzer.Key, context.SourceCodeVolumeId, context.ImportsVolumeId, analyzer.Value))
                    .ToArray();

                var analyzerDetails = await Task.WhenAll(analyzerContainerTasks);

                foreach (var (analyzerName, _, artifactVolume) in analyzerDetails)
                {
                    _logger.LogInformation("Finalizing activity: {activity}", analyzerName);

                    await CopyVolumeFilesAsync(artifactVolume, context.ArtifactsVolumeId, analyzerName);
                }

                return analyzerDetails.All(x => x.exitCode == 0L);
            }
            finally
            {
                await CleanUpVolumesAsync();
            }
        }

        private async Task CopyVolumeFilesAsync(string sourceVolumeId, string dstVolumeId, string subFolder = null)
        {
            var dstFolder = string.IsNullOrWhiteSpace(subFolder) ? "/dst/" : $"/dst/{subFolder}/";

            var activityManifest = new ActivityManifest
            {
                Image = _workerImageName,
                Command = new[] { "cp", "-a", "/src/.", dstFolder },
                Volumes = new VolumesManifest
                {
                    Sources = "/src",
                    Artifacts = "/dst"
                }
            };

            await _activityExecutor.ExecuteAsync(
                activityManifest,
                false,
                sourceVolumeId,
                dstVolumeId,
                null);
        }

        private async Task CleanUpVolumesAsync()
        {
            foreach (var volume in _volumesToCleanUp.ToArray())
            {
                await _containerService.RemoveVolumeAsync(volume);
            }
        }

        private async Task<(string analyzerName, long exitCode, string artifactVolume)>
            RunAnalyzerAsync(
                string analyzerName,
                string srcVolumeId,
                string importsVolumeId,
                ActivityManifest manifest)
        {
            var analyzerSrcVolumeId = await _containerService.CreateVolumeAsync("a-src");
            AddVolumeToCleanUpList(analyzerSrcVolumeId);

            var analyzerArtifactVolumeId = await _containerService.CreateVolumeAsync("a-art");
            AddVolumeToCleanUpList(analyzerArtifactVolumeId);

            var analyzerImportsVolumeId = await _containerService.CreateVolumeAsync("a-imp");
            AddVolumeToCleanUpList(analyzerImportsVolumeId);

            await CopyVolumeFilesAsync(srcVolumeId, analyzerSrcVolumeId);
            await CopyVolumeFilesAsync(importsVolumeId, analyzerImportsVolumeId);

            var result = await _activityExecutor.ExecuteAsync(
                manifest,
                true,
                analyzerSrcVolumeId,
                analyzerArtifactVolumeId,
                analyzerImportsVolumeId
            );

            return (analyzerName, result.ExitCode, analyzerArtifactVolumeId);
        }

        private void AddVolumeToCleanUpList(string volumeId)
        {
            _volumesToCleanUp.Add(volumeId);
        }
    }
}