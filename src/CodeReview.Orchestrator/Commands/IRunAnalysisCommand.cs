using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface IRunAnalysisCommand
    {
        Task<int> ExecuteAsync(string manifestPath);
    }
}