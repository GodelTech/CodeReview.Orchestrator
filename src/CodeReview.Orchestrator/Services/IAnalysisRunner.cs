using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IAnalysisRunner
    {
        Task<int> RunAsync(string manifestPath);
    }
}