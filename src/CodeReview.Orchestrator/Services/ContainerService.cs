using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace CodeReview.Orchestrator.Services
{
    public class ContainerService : IContainerService
    {
        private readonly IDockerClientFactory _dockerClientFactory;
        private readonly INameFactory _nameFactory;

        public ContainerService(
            IDockerClientFactory dockerClientFactory,
            INameFactory nameFactory)
        {
            _dockerClientFactory = dockerClientFactory ?? throw new ArgumentNullException(nameof(dockerClientFactory));
            _nameFactory = nameFactory ?? throw new ArgumentNullException(nameof(nameFactory));
        }

        public async Task<string> CreateContainerAsync(
            string imageName,
            string[] commandLineArgs,
            IReadOnlyDictionary<string, string> envVariables,
            params MountedVolume[] volumes)
        {
            if (volumes == null)
                throw new ArgumentNullException(nameof(volumes));
            if (string.IsNullOrWhiteSpace(imageName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(imageName));

            var mounts =
                (from volume in volumes
                    select new Mount
                    {
                        ReadOnly = volume.IsReadOnly,
                        Source = volume.Source,
                        Target = volume.Target,
                        Type = volume.IsBindMount ? "bind" : "volume"
                    })
                .ToArray();

            using (var client = _dockerClientFactory.Create())
            {
                var createResult = await client.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = imageName,
                    Env = (envVariables ?? ImmutableDictionary<string, string>.Empty).Select(x => x.Key + "=" + x.Value)
                        .ToArray(),
                    HostConfig = new HostConfig
                    {
                        Mounts = mounts
                    },
                    Cmd = commandLineArgs
                });

                return createResult.ID;
            }
        }

        public async Task WaitContainer(string containerId, long waitTimeoutSeconds = 900)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));
            if (waitTimeoutSeconds <= 0) 
                throw new ArgumentOutOfRangeException(nameof(waitTimeoutSeconds));

            using (var client = _dockerClientFactory.Create())
            using (var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(waitTimeoutSeconds)))
            {
                await client.Containers.WaitContainerAsync(containerId, tokenSource.Token);
            }
        }

        public async Task ImportFilesIntoContainerAsync(string containerId, string containerPath, Stream inStream)
        {
            if (inStream == null) 
                throw new ArgumentNullException(nameof(inStream));
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));
            if (string.IsNullOrWhiteSpace(containerPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerPath));

            using (var client = _dockerClientFactory.Create())
            {
                await client.Containers.ExtractArchiveToContainerAsync(
                    containerId,
                    new ContainerPathStatParameters
                    {
                        Path = containerPath,
                        AllowOverwriteDirWithFile = true
                    },
                    inStream);
            }
        }

        public async Task ExportFilesFromContainerAsync(string containerId, string containerPath, Stream outStream)
        {
            if (outStream == null) 
                throw new ArgumentNullException(nameof(outStream));
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));
            if (string.IsNullOrWhiteSpace(containerPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerPath));

            using (var client = _dockerClientFactory.Create())
            {
                var result = await client.Containers.GetArchiveFromContainerAsync(
                    containerId,
                    new GetArchiveFromContainerParameters
                    {
                        Path = containerPath
                    },
                    false);

                await result.Stream.CopyToAsync(outStream);
            }
        }

        public async Task StartContainerAsync(string containerId)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));

            using (var client = _dockerClientFactory.Create())
            {
                await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
            }
        }

        public async Task StopContainerAsync(string containerId, int waitBeforeKillSeconds = 30)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));
            if (waitBeforeKillSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(waitBeforeKillSeconds));

            using (var client = _dockerClientFactory.Create())
            {
                await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters
                {
                    WaitBeforeKillSeconds = (uint) waitBeforeKillSeconds
                });
            }
        }

        public async Task RemoveContainerAsync(string containerId, bool force = false)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));

            using (var client = _dockerClientFactory.Create())
            {
                await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters
                {
                    Force = force
                });
            }
        }

        public async Task<ContainerLogs> GetContainerLogsAsync(string containerId)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));

            using (var client = _dockerClientFactory.Create())
            {
                var contentStream = await client.Containers.GetContainerLogsAsync(containerId, false,
                    new ContainerLogsParameters
                    {
                        ShowStderr = true,
                        ShowStdout = true,
                        Timestamps = false
                    });

                using (contentStream)
                using (var stdOutStream = new MemoryStream())
                using (var stdInStream = new MemoryStream())
                using (var stdErrStream = new MemoryStream())
                {
                    await contentStream.CopyOutputToAsync(stdInStream, stdOutStream, stdErrStream, default);

                    stdOutStream.Position = 0;

                    using (var stdOutReader = new StreamReader(stdOutStream))
                    using (var stdErrReader = new StreamReader(stdErrStream))
                    {
                        return new ContainerLogs
                        {
                            StdErr = await stdErrReader.ReadToEndAsync(),
                            StdOut = await stdOutReader.ReadToEndAsync()
                        };
                    }
                }
            }
        }

        public async Task<string> CreateVolumeAsync(string volumePrefix)
        {
            if (string.IsNullOrWhiteSpace(volumePrefix))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(volumePrefix));

            using (var client = _dockerClientFactory.Create())
            {

                var result = await client.Volumes.CreateAsync(new VolumesCreateParameters
                {
                    Name = _nameFactory.CreateVolumeName(volumePrefix)
                });

                return result.Name;
            }
        }

        public async Task RemoveVolumeAsync(string volumeId)
        {
            if (string.IsNullOrWhiteSpace(volumeId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(volumeId));

            using (var client = _dockerClientFactory.Create())
            {
                await client.Volumes.RemoveAsync(volumeId);
            }
        }


        public async Task<ContainerInfo> GetContainerInfo(string containerId)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));

            using (var client = _dockerClientFactory.Create())
            {
                try
                {
                    var container = await client.Containers.InspectContainerAsync(containerId);
                    if (container == null)
                        return null;

                    return new ContainerInfo
                    {
                        Id = container.ID,
                        Status = container.State.Status,
                        ExitCode = container.State.ExitCode
                    };
                }
                catch (DockerContainerNotFoundException)
                {
                    return null;
                }
            }
        }
    }
}
