using ReviewItEasy.Orchestrator.Model;

namespace ReviewItEasy.Orchestrator.Commands
{
    public interface IActivityFactory
    {
        IActivity Create(AnalysisManifest manifest);
    }
}