using Docker.DotNet;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerClientFactory : IDockerClientFactory
    {
        public IDockerClient Create()
        {
            return new DockerClientConfiguration().CreateClient();
        }
    }
}