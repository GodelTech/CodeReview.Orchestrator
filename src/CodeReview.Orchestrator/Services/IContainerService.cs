﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IContainerService
    {
        Task<string> CreateVolumeAsync(string volumePrefix);
        Task RemoveVolumeAsync(string volumeId);
        Task<string> CreateContainerAsync(string imageName, params MountedVolume[] volumes);
        Task<string> CreateContainerAsync(string imageName, string[] commandLineArgs, IReadOnlyDictionary<string, string> envVariables, params MountedVolume[] volumes);
        Task StartContainerAsync(string containerId);
        Task StopContainerAsync(string containerId, int waitBeforeKillSeconds = 30);
        Task RemoveContainerAsync(string containerId, bool force = false);
        Task<IContainerLogListener> AttachToContainerStream(string containerId, long waitTimeoutSeconds = 900);
        Task<ContainerInfo> GetContainerInfo(string containerId);
        Task WaitContainer(string containerId, long waitTimeoutSeconds = 900);
        Task ExportFilesFromContainerAsync(string containerId, string containerPath, Stream outStream);
        Task ImportFilesIntoContainerAsync(string containerId, string containerPath, Stream inStream);
        Task<bool> ImageExists(string imageName);
        Task PullImageAsync(string imageName);
    }
}