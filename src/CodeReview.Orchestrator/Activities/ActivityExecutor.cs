﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
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
            
            _logger.LogInformation("Current Docker Engine: {engine}", _engineContext.Engine.Name);
            
            var mounts = ResolveMountedVolumes(manifest.Volumes, context.SourceCodeVolumeId, context.ArtifactsVolumeId, context.ImportsVolumeId).ToArray();

            _logger.LogInformation("Creating container. Image = {imageName}...", manifest.Image);
            var containerId = await _containerService.CreateContainerAsync(
                manifest.Image,
                manifest.Command,
                manifest.Environment,
                mounts);
            _logger.LogInformation("Container created.");


            try
            {
                _logger.LogInformation("Starting container. ContainerId = {containerId}...", containerId);
                await _containerService.StartContainerAsync(containerId);
                _logger.LogInformation("Container started. ContainerId = {containerId}", containerId);

                _logger.LogInformation("Waiting for container. ContainerId = {containerId}...", containerId);
                await _containerService.WaitContainer(containerId, manifest.Settings.WaitTimeoutSeconds);
                _logger.LogInformation("Container exited. ContainerId = {containerId}", containerId);

                var logDetails = new ContainerLogs();
                if (readLogs)
                    logDetails = await _containerService.GetContainerLogsAsync(containerId);

                var info = await _containerService.GetContainerInfo(containerId);
                var exitCode = info.ExitCode;

                _logger.LogInformation("Container={containerId}, ExitCode={exitCode}", containerId, exitCode);
                _logger.LogInformation("Container={containerId} Stdout: {stdOut}", containerId, logDetails.StdOut);
                if (!string.IsNullOrWhiteSpace(logDetails.StdErr))
                    _logger.LogError("Container={containerId} Stderr: {stdErr}", containerId, logDetails.StdErr);

                return new ExecutionResult
                {
                    StdErr = logDetails.StdErr,
                    StdOut = logDetails.StdOut,
                    ExitCode = exitCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run container.");
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

        private IEnumerable<MountedVolume> ResolveMountedVolumes(VolumesManifest mount, string srcVolumeId,
            string artifactsVolumeId, string importsVolumeId)
        {
            if (!string.IsNullOrWhiteSpace(srcVolumeId))
                yield return new MountedVolume
                {
                    IsBindMount = false,
                    Source = srcVolumeId,
                    Target = mount.Sources
                };

            if (!string.IsNullOrWhiteSpace(artifactsVolumeId))
                yield return new MountedVolume
                {
                    IsBindMount = false,
                    Source = artifactsVolumeId,
                    Target = mount.Artifacts
                };

            if (!string.IsNullOrWhiteSpace(importsVolumeId))
                yield return new MountedVolume
                {
                    IsBindMount = false,
                    IsReadOnly = true,
                    Source = importsVolumeId,
                    Target = mount.Imports
                };
        }
    }
}