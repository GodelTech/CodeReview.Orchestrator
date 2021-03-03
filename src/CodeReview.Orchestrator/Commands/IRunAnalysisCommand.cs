using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Options;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface IRunAnalysisCommand
    {
        Task<int> ExecuteAsync(RunOptions options);
    }
}