using ReviewItEasy.Orchestrator.Model;

namespace ReviewItEasy.Orchestrator.Services
{
    public interface IManifestValidator
    {
        bool IsValid(AnalysisManifest manifest);
    }
}