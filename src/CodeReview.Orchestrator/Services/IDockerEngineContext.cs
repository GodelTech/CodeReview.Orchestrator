using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IDockerEngineContext
    {
        DockerEngine Engine { get; set; }
    }
}