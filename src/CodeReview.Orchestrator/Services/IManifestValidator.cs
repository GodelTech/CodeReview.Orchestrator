using CodeReview.Orchestrator.Model;

namespace CodeReview.Orchestrator.Services
{
    public interface IManifestValidator
    {
        bool IsValid(AnalysisManifest manifest);
    }
}