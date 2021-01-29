using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IManifestValidationRunner
    {
        Task<int> RunAsync(string manifestPath, string outputFilePath);
    }
}