using ReviewItEasy.Orchestrator.Model;

namespace ReviewItEasy.Orchestrator.Services
{
    public interface IManifestExpressionExpander
    {
        void Expand(AnalysisManifest manifest);
    }
}