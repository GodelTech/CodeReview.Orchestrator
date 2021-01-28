using System.Threading.Tasks;

namespace ReviewItEasy.Orchestrator.Services
{
    public interface IAnalysisRunner
    {
        Task<int> RunAsync(string manifestPath);
    }
}