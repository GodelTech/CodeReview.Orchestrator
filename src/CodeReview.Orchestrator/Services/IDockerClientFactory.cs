using Docker.DotNet;

namespace CodeReview.Orchestrator.Services
{
    public interface IDockerClientFactory
    {
        IDockerClient Create();
    }
}