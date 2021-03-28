using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Options;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface ICreateManifestCommand
    {
        Task<int> ExecuteAsync(NewAnalysisManifestOptions options);
    }
}