using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReviewItEasy.Orchestrator.Model;
using ReviewItEasy.Orchestrator.Services;

namespace ReviewItEasy.Orchestrator.Commands
{
    public class ActivityExecutor : IActivityExecutor
    {
        private readonly IContainerService _containerService;
        private readonly ILogger<ActivityExecutor> _logger;

        public ActivityExecutor(
            IContainerService containerService,
            ILogger<ActivityExecutor> logger)
        {
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _logger = logger;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            ActivityManifest manifest,
            bool readLogs,
            string srcVolumeId,
            string artifactsVolumeId,
            string importsVolumeId)
        {
            var mounts = ResolveMountedVolumes(manifest.Volumes, srcVolumeId, artifactsVolumeId, importsVolumeId).ToArray();

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