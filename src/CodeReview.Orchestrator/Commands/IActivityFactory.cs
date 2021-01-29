using CodeReview.Orchestrator.Model;

namespace CodeReview.Orchestrator.Commands
{
    public interface IActivityFactory
    {
        IActivity Create(AnalysisManifest manifest);
    }
}