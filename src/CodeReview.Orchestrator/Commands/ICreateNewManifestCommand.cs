using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface ICreateNewManifestCommand
    {
        Task<int> ExecuteAsync(string filePath);
    }
}