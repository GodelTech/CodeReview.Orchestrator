using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Options;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface ICreateDockerEngineCollectionManifestCommand
    {
        Task<int> ExecuteAsync(NewDockerEngineCollectionManifestOptions options);
    }
}