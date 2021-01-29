using Docker.DotNet;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IDockerClientFactory
    {
        IDockerClient Create();
    }
}