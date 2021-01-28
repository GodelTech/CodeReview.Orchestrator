using Docker.DotNet;

namespace ReviewItEasy.Orchestrator.Services
{
    public class DockerClientFactory : IDockerClientFactory
    {
        public IDockerClient Create()
        {
            return new DockerClientConfiguration().CreateClient();
        }
    }
}