using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public interface IActivityFactory
    {
        IActivity Create(AnalysisManifest manifest);
    }
}