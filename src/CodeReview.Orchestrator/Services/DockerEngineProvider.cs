using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerEngineProvider : IDockerEngineProvider
    {
        private readonly IManifestProvider _manifestProvider;
        private readonly IManifestValidator _manifestValidator;
        private readonly List<DockerEngine> _engines = new();

        public DockerEngineProvider(
            IManifestProvider manifestProvider,
            IManifestValidator manifestValidator)
        {
            _manifestProvider = manifestProvider ?? throw new ArgumentNullException(nameof(manifestProvider));
            _manifestValidator = manifestValidator ?? throw new ArgumentNullException(nameof(manifestValidator));
        }

        public async Task InitializeAsync(string manifestFilePath)
        {
            if (string.IsNullOrWhiteSpace(manifestFilePath))
            {
                _engines.Add(new DockerEngine
                {
                    Name = "Default",
                    WorkerImage = "alpine",
                    IsDefault = true,
                    Os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? OperatingSystemType.Windows : OperatingSystemType.Linux,
                    Url = LocalDockerUri(),
                    Features = Array.Empty<string>()
                });

                return;
            }

            var manifest = await _manifestProvider.GetAsync<DockerEngineCollectionManifest>(manifestFilePath);
            if (manifest == null)
                throw new InvalidOperationException("Failed to load Docker Engine collection manifest");

            if (!_manifestValidator.IsValid(manifest))
                throw new InvalidOperationException("Docker Engine collection manifest is invalid");

            foreach (var (name, engineManifest) in manifest.Engines)
            {
                _engines.Add(new DockerEngine
                {
                    Os = engineManifest.Os,
                    Features = engineManifest.Features ?? Array.Empty<string>(),
                    WorkerImage = engineManifest.WorkerImage,
                    IsDefault = manifest.DefaultEngine.Equals(name, StringComparison.OrdinalIgnoreCase),
                    Name = name,
                    Url = engineManifest.Url,
                    ResourceLabel = engineManifest.ResourceLabel,
                    AuthType = engineManifest.AuthType,
                    BasicAuthCredentials = engineManifest.BasicAuthCredentials,
                    CertificateCredentials = engineManifest.CertificateCredentials
                });
            }
        }

        public IEnumerable<DockerEngine> ListAll()
        {
            return _engines;
        }

        public DockerEngine Find(RequirementsManifest requirements)
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));

            var engineQuery = _engines.AsQueryable();

            if (requirements.Os.HasValue)
                engineQuery = engineQuery.Where(x => x.Os == requirements.Os.Value);

            if (requirements.Features.Any())
                engineQuery = engineQuery.Where(x =>
                    requirements.Features.All(p => x.Features.Contains(p, StringComparer.OrdinalIgnoreCase)));

            if (!requirements.Os.HasValue && !requirements.Features.Any())
                engineQuery = engineQuery.Where(x => x.IsDefault);

            return engineQuery.FirstOrDefault();
        }

        private static string LocalDockerUri()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ? "npipe://./pipe/docker_engine" : "unix:/var/run/docker.sock";
        }
    }
}