using System.Threading.Tasks;

namespace CodeReview.Orchestrator.Services
{
    public interface IAnalysisRunner
    {
        Task<int> RunAsync(string manifestPath);
    }
}