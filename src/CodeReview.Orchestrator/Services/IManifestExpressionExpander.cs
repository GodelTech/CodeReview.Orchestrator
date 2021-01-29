using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IManifestExpressionExpander
    {
        void Expand(AnalysisManifest manifest);
    }
}