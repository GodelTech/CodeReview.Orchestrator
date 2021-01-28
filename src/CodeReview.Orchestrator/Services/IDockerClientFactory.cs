using Docker.DotNet;

namespace ReviewItEasy.Orchestrator.Services
{
    public interface IDockerClientFactory
    {
        IDockerClient Create();
    }
}