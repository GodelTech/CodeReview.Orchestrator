using CodeReview.Orchestrator.Model;

namespace CodeReview.Orchestrator.Services
{
    public interface IManifestExpressionExpander
    {
        void Expand(AnalysisManifest manifest);
    }
}