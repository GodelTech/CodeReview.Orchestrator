using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class ContainerService : IContainerService
    {
        private class NullProgress: IProgress<JSONMessage>
        {
            public void Report(JSONMessage value) {
            }
        }

        private readonly IDockerClientFactory _dockerClientFactory;
        private readonly IDockerEngineContext _dockerEngineContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly INameFactory _nameFactory;

        public ContainerService(
            IDockerClientFactory dockerClientFactory,
            IDockerEngineContext dockerEngineContext,
            ILoggerFactory loggerFactory,
            INameFactory nameFactory)
        {
            _dockerClientFactory = dockerClientFactory ?? throw new ArgumentNullException(nameof(dockerClientFactory));
            _dockerEngineContext = dockerEngineContext ?? throw new ArgumentNullException(nameof(dockerEngineContext));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _nameFactory = nameFactory ?? throw new ArgumentNullException(nameof(nameFactory));
        }

        public async Task<bool> ImageExists(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(imageName));
            
            using var client = _dockerClientFactory.Create();

            // See Docker REST API for more details https://docs.docker.com/engine/api/v1.41/#operation/ImageList
            var imageListParams = new ImagesListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["reference"] = new Dictionary<string, bool>
                    {
                        [imageName] = true
                    }
                }
            };
            
            var matches = await client.Images.ListImagesAsync(imageListParams);
            
            return matches.Any();
        }

        public async Task PullImageAsync(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(imageName));

            var tag = "latest";
            var imageWithTag = imageName;

            if (imageName.Contains(":"))
            {
                tag = imageName.Substring(imageName.IndexOf(':') + 1);
                imageWithTag = imageName.Substring(0, imageWithTag.IndexOf(':'));
            }

            using var client = _dockerClientFactory.Create();
            
            var parameters = new ImagesCreateParameters
            {
                FromImage = imageWithTag,
                Tag = tag
            };

            await client.Images.CreateImageAsync(parameters, null, new NullProgress());
        }

        public Task<string> CreateContainerAsync(string imageName, params MountedVolume[] volumes)
            => CreateContainerAsync(imageName, Array.Empty<string>(), ImmutableDictionary<string, string>.Empty, volumes);
        
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

            using var client = _dockerClientFactory.Create();

            var parameters = new CreateContainerParameters
            {
                Image = imageName,
                Env = (envVariables ?? ImmutableDictionary<string, string>.Empty).Select(x => x.Key + "=" + x.Value)
                    .ToArray(),
                HostConfig = new HostConfig
                {
                    Mounts = mounts
                },
                Cmd = commandLineArgs,
                Labels = new Dictionary<string, string>()
            };

            var resourceLabel = _dockerEngineContext.Engine.ResourceLabel;
            if (!string.IsNullOrWhiteSpace(resourceLabel))
                parameters.Labels[resourceLabel] = string.Empty;
            
            var createResult = await client.Containers.CreateContainerAsync(parameters);

            return createResult.ID;
        }

        public async Task WaitContainer(string containerId, long waitTimeoutSeconds = 900)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));
            if (waitTimeoutSeconds <= 0) 
                throw new ArgumentOutOfRangeException(nameof(waitTimeoutSeconds));

            using var client = _dockerClientFactory.Create();
            using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(waitTimeoutSeconds));
            
            await client.Containers.WaitContainerAsync(containerId, tokenSource.Token);
        }

        public async Task ImportFilesIntoContainerAsync(string containerId, string containerPath, Stream inStream)
        {
            if (inStream == null) 
                throw new ArgumentNullException(nameof(inStream));
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));
            if (string.IsNullOrWhiteSpace(containerPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerPath));

            using var client = _dockerClientFactory.Create();

            await client.Containers.ExtractArchiveToContainerAsync(
                containerId,
                new ContainerPathStatParameters
                {
                    Path = containerPath,
                    AllowOverwriteDirWithFile = true
                },
                inStream);
        }

        public async Task ExportFilesFromContainerAsync(string containerId, string containerPath, Stream outStream)
        {
            if (outStream == null) 
                throw new ArgumentNullException(nameof(outStream));
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));
            if (string.IsNullOrWhiteSpace(containerPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerPath));

            using var client = _dockerClientFactory.Create();
            
            var result = await client.Containers.GetArchiveFromContainerAsync(
                containerId,
                new GetArchiveFromContainerParameters
                {
                    Path = containerPath
                },
                false);

            await result.Stream.CopyToAsync(outStream);
        }

        public async Task StartContainerAsync(string containerId)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));

            using var client = _dockerClientFactory.Create();
            
            await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
        }

        public async Task StopContainerAsync(string containerId, int waitBeforeKillSeconds = 30)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));
            if (waitBeforeKillSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(waitBeforeKillSeconds));

            using var client = _dockerClientFactory.Create();
            
            await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters
            {
                WaitBeforeKillSeconds = (uint) waitBeforeKillSeconds
            });
        }

        public async Task RemoveContainerAsync(string containerId, bool force = false)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));

            using var client = _dockerClientFactory.Create();
            
            await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters
            {
                Force = force
            });
        }

        public async Task<IContainerLogListener> AttachToContainerStream(string containerId, long waitTimeoutSeconds = 900)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));

            using var client = _dockerClientFactory.Create();

            var stream = await client.Containers.AttachContainerAsync(containerId, false, new ContainerAttachParameters
            {
                Stdout = true,
                Stderr = true,
                Stream = true
            });

            return new ContainerLogListener(
                _loggerFactory.CreateLogger<ContainerLogListener>(),
                containerId,
                stream);
        }

        public async Task<string> CreateVolumeAsync(string volumePrefix)
        {
            if (string.IsNullOrWhiteSpace(volumePrefix))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(volumePrefix));

            using var client = _dockerClientFactory.Create();

            var parameters = new VolumesCreateParameters
            {
                Name = _nameFactory.CreateVolumeName(volumePrefix),
                Labels = new Dictionary<string, string>()
            };
            
            var resourceLabel = _dockerEngineContext.Engine.ResourceLabel;
            if (!string.IsNullOrWhiteSpace(resourceLabel)) 
                parameters.Labels[resourceLabel] = string.Empty;

            var result = await client.Volumes.CreateAsync(parameters);

            return result.Name;
        }

        public async Task RemoveVolumeAsync(string volumeId)
        {
            if (string.IsNullOrWhiteSpace(volumeId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(volumeId));

            using var client = _dockerClientFactory.Create();
            
            await client.Volumes.RemoveAsync(volumeId);
        }


        public async Task<ContainerInfo> GetContainerInfo(string containerId)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerId));

            using var client = _dockerClientFactory.Create();
            
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