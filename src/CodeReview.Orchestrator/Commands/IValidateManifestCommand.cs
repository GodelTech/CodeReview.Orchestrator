using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface IValidateManifestCommand
    {
        Task<int> ExecuteAsync(string manifestPath, string outputFilePath);
    }
}