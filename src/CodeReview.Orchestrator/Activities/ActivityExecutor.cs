using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using GodelTech.CodeReview.Orchestrator.Utils;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ActivityExecutor : IActivityExecutor
    {
        private readonly IDockerContextSwitcher _dockerContextSwitcher;
        private readonly IContainerService _containerService;
        private readonly IDockerEngineContext _engineContext;
        private readonly ILogger<ActivityExecutor> _logger;

        public ActivityExecutor(
            IDockerContextSwitcher dockerContextSwitcher,
            IContainerService containerService,
            IDockerEngineContext engineContext,
            ILogger<ActivityExecutor> logger)
        {
            _dockerContextSwitcher = dockerContextSwitcher ?? throw new ArgumentNullException(nameof(dockerContextSwitcher));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ExecutionResult> ExecuteAsync(
            ActivityManifest manifest,
            bool readLogs,
            IProcessingContext context)
        {
            await _dockerContextSwitcher.SwitchAsync(context, manifest.Requirements);

            OverrideVolumes(context, manifest);

            _logger.LogInformation("Current Docker Engine: {engine}", _engineContext.Engine.Name);

            var mounts = ResolveMountedVolumes(context).ToArray();

            _logger.LogInformation("Creating container. Image = {imageName}...", manifest.Image);
            var containerId = await _containerService.CreateContainerAsync(
                manifest.Image,
                manifest.Command,
                manifest.Environment,
                mounts);
            _logger.LogInformation("Container created.");

            try
            {
                await using var _ = await AttachedToContainerAsync(readLogs, containerId, manifest.Settings.WaitTimeoutSeconds);

                _logger.LogInformation("Starting container. ContainerId = {containerId}...", containerId);

                await _containerService.StartContainerAsync(containerId);

                _logger.LogInformation("Container started. ContainerId = {containerId}", containerId);

                _logger.LogInformation("Waiting for container. ContainerId = {containerId}...", containerId);

                await _containerService.WaitContainer(containerId, manifest.Settings.WaitTimeoutSeconds);

                _logger.LogInformation("Container exited. ContainerId = {containerId}", containerId);

                var info = await _containerService.GetContainerInfo(containerId);
                var exitCode = info.ExitCode;

                return new ExecutionResult
                {
                    ExitCode = exitCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to run container. {ex.Message}");
                throw;
            }
            finally
            {
                _logger.LogInformation("Stopping container. ContainerId = {containerId}...", containerId);
                await _containerService.StopContainerAsync(containerId);
                _logger.LogInformation("Container stopped. ContainerId = {containerId}", containerId);

                _logger.LogInformation("Removing container. ContainerId = {containerId}...", containerId);
                await _containerService.RemoveContainerAsync(containerId);
                _logger.LogInformation("Container removed. ContainerId = {containerId}", containerId);
            }
        }

        private async Task<IAsyncDisposable> AttachedToContainerAsync(bool readLogs, string containerId, long waitTimeoutSeconds)
        {
            if (!readLogs)
                return Disposable.AsyncEmpty;

            _logger.LogInformation("Attaching to the container. ContainerId = {containerId}...", containerId);
            var logListener = await _containerService.AttachToContainerStream(containerId, waitTimeoutSeconds);
            _logger.LogInformation("Attached to the container. ContainerId = {containerId}...", containerId);

            return logListener;
        }

        private static void OverrideVolumes(IProcessingContext context, ActivityManifest activityManifest)
        {
            foreach (var volume in context.Volumes)
            {
                if (activityManifest.Volumes.TryGetValue(volume.Name, out var activityVolume))
                    volume.TargetFolder = activityVolume.TargetFolder;
            }
        }

        private static IEnumerable<MountedVolume> ResolveMountedVolumes(IProcessingContext context)
        {
            return context.Volumes.Select(volume => new MountedVolume
            {
                IsBindMount = false,
                Source = context.ResolveVolumeId(volume.Name),
                Target = volume.TargetFolder,
                IsReadOnly = volume.ReadOnly
            });
        }
    }
}