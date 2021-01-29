using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface IActivityFactory
    {
        IActivity Create(AnalysisManifest manifest);
    }
}