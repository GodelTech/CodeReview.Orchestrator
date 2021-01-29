using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IManifestValidator
    {
        bool IsValid(AnalysisManifest manifest);
    }
}