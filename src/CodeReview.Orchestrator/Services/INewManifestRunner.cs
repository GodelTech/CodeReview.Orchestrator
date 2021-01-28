using System.Threading.Tasks;

namespace ReviewItEasy.Orchestrator.Services
{
    public interface INewManifestRunner
    {
        Task<int> RunAsync(string filePath);
    }
}