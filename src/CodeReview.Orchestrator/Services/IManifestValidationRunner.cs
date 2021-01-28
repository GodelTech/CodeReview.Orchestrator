using System.Threading.Tasks;

namespace ReviewItEasy.Orchestrator.Services
{
    public interface IManifestValidationRunner
    {
        Task<int> RunAsync(string manifestPath, string outputFilePath);
    }
}