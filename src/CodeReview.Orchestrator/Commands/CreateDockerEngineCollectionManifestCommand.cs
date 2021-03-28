using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class CreateDockerEngineCollectionManifestCommand : ICreateDockerEngineCollectionManifestCommand
    {
        private readonly IFileService _fileService;
        private readonly IYamlSerializer _yamlSerializer;

        public CreateDockerEngineCollectionManifestCommand(IFileService fileService, IYamlSerializer yamlSerializer)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
        }

        public async Task<int> ExecuteAsync(NewDockerEngineCollectionManifestOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));


            var manifest = new DockerEngineCollectionManifest
            {
                DefaultEngine = "default",
                Engines = new ()
                {
                    ["default"] = new()
                    {
                        Os = OperatingSystemType.Linux,
                        WorkerImage = "alpine",
                        Url = "unix:///var/run/docker.sock",
                        Features = new[]
                        {
                            "feature1",
                            "feature2"
                        }
                    },
                    ["windows"] = new()
                    {
                        Os = OperatingSystemType.Windows,
                        WorkerImage = "alpine",
                        Url = "npipe:////./pipe/docker_engine",
                        Features = new[]
                        {
                            "feature1",
                            "feature2"
                        }   
                    },
                    ["remote"] = new()
                    {
                        Os = OperatingSystemType.Windows,
                        WorkerImage = "alpine",
                        Url = "http://remote.api:2375/",
                        Features = new[]
                        {
                            "feature1",
                            "feature2"
                        }
                    }
                }
            };

            var yaml = _yamlSerializer.Serialize(manifest);

            await _fileService.WriteAllTextAsync(options.File, yaml);

            return Constants.SuccessExitCode;
        }
    }
}
